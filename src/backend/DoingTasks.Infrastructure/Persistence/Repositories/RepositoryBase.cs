using DoingTasks.Domain;
using DoingTasks.SharedKernel.Domain;
using Microsoft.EntityFrameworkCore;

namespace DoingTasks.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
    where TEntity : Entity
{
    protected readonly ApplicationDbContext _context;

    protected RepositoryBase(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Set<TEntity>().FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public void Add(TEntity entity)
    {
        _context.Set<TEntity>().Add(entity);
    }

    public void Update(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }
}
