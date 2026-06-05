using DoingTasks.Domain.Auditing;

namespace DoingTasks.Infrastructure.Persistence.Repositories;

internal sealed class WorkspaceAuditLogRepository : RepositoryBase<WorkspaceAuditLog>, IWorkspaceAuditLogRepository
{
    public WorkspaceAuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }
}
