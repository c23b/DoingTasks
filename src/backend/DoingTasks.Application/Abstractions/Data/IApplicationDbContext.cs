using DoingTasks.Domain.Auditing;
using DoingTasks.Domain.Tasks;
using DoingTasks.Domain.Users;
using DoingTasks.Domain.Workspaces;
using Microsoft.EntityFrameworkCore;
namespace DoingTasks.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Workspace> Workspaces { get; }
    DbSet<WorkTask> WorkTasks { get; }
    DbSet<WorkspaceAuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
