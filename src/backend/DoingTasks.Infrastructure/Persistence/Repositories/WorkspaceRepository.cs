using DoingTasks.Domain.Workspaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoingTasks.Infrastructure.Persistence.Repositories;

internal sealed class WorkspaceRepository : RepositoryBase<Workspace>, IWorkspaceRepository
{
    public WorkspaceRepository(ApplicationDbContext context) : base(context)
    {
    }
}
