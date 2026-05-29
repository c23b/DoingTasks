using DoingTasks.Domain.Tasks;
namespace DoingTasks.Domain.UnitTests.Tasks;

[CollectionDefinition(nameof(WorkTaskCollection))]
public class WorkTaskCollection : ICollectionFixture<WorkTaskTestFixture> { }

public class WorkTaskTestFixture : IDisposable
{

    public WorkTaskTestFixture()
    {

    }
    /// <summary>
    /// Returns a valid work task for testing purposes.
    /// </summary>
    /// <returns></returns>
    public WorkTask GenerateWorkTask()
    {
        return WorkTask.Create(
            "Task",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Task DEscription",
            1,
            Guid.NewGuid(),
            1).Value;
    }

    public WorkTask GenerateWorkTask(
        string title,
        Guid workspaceId,
        Guid initialStateId,
        string? description = null,
        int? plannedHours = null,
        Guid? assignedUserId = null,
        int? complexity = null)
    {
        return WorkTask.Create(
            title,
            workspaceId,
            initialStateId,
            description,
            plannedHours,
            assignedUserId,
            complexity).Value;
    }


    public void Dispose()
    {

    }
}

