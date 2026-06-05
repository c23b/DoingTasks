using DoingTasks.Domain.Tasks;

namespace DoingTasks.Infrastructure.Persistence.Repositories;

internal sealed class WorkTaskRepository : RepositoryBase<WorkTask>, IWorkTaskRepository
{
    public WorkTaskRepository(ApplicationDbContext context) : base(context)
    {
    }
}
