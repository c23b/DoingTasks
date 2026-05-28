namespace DoingTasks.Domain.WorkspaceTests;

[Collection(nameof(WorkspaceCollection))]
public class WorkspaceTest
{
    private readonly WorkspaceTestFixture _workspaceTestFixture;

    public WorkspaceTest(WorkspaceTestFixture workspaceTestFixture)
    {
        _workspaceTestFixture = workspaceTestFixture;
    }

    [Fact]
    public void Test1()
    {

    }
}
