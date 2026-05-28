using DoingTasks.Domain.Workspaces;
namespace DoingTasks.Domain.WorkspaceTests;

[CollectionDefinition(nameof(WorkspaceCollection))]
public class WorkspaceCollection : ICollectionFixture<WorkspaceTestFixture> { }

public class WorkspaceTestFixture : IDisposable
{

    public WorkspaceTestFixture()
    {

    }
    /// <summary>
    /// Returns a valid user for testing purposes. New Guid, Workspace, Group
    /// </summary>
    /// <returns></returns>
    public Workspace GenerateWorkspace()
    {
        return Workspace.Create(
            Guid.NewGuid(),
            "Workspace",
            "Group").Value;
    }

    public Workspace GenerateWorkspace(
        Guid ownerId,
        string name,
        string? groupName = null,
        bool allowCollaboratorEditing = false)
    {
        return Workspace.Create(
            ownerId,
            name,
            groupName,
            allowCollaboratorEditing).Value;
    }


    public void Dispose()
    {

    }
}