using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;
public interface IRepositoryBase<TEntity>
    where TEntity : Entity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}
