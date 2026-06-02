using DoingTasks.Application.Abstractions.Data;
using DoingTasks.Domain.Auditing;
using DoingTasks.Domain.Tasks;
using DoingTasks.Domain.Users;
using DoingTasks.Domain.Workspaces;
using DoingTasks.Infrastructure.DomainEvents;
using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection;

namespace DoingTasks.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDomainEventDispatcher domainEventDispatcher,
        IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
        _dateTimeProvider = dateTimeProvider;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<WorkTask> WorkTasks => Set<WorkTask>();
    public DbSet<WorkspaceAuditLog> AuditLogs => Set<WorkspaceAuditLog>();

    static ApplicationDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    public async Task<bool> Commit(CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();

        var domainEvents = GetDomainEvents();

        var result = await base.SaveChangesAsync(cancellationToken) > 0;

        if (result)
            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);

        return result;
    }

    private void UpdateAuditableEntities()
    {
        var now = _dateTimeProvider.UtcNow;

        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.SetCreatedAt(now);

            if (entry.State == EntityState.Modified)
                entry.Entity.SetUpdatedAt(now);
        }
    }

    private IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .SelectMany(e =>
            {
                var events = e.DomainEvents.ToList();
                e.ClearDomainEvents();
                return events;
            })
            .ToList();
    }
}
