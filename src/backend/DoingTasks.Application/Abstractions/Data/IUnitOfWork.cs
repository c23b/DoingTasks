
namespace DoingTasks.Application.Abstractions.Data;

public interface IUnitOfWork
{
    Task<bool> Commit(CancellationToken cancellationToken = default);
}
