using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Task;

public sealed class TaskComment : Entity
{
    public string Content { get; private set; }
    public Guid AuthorId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TaskComment() { }

    internal static Result<TaskComment> Create(string content, Guid authorId)
    {
        if (string.IsNullOrWhiteSpace(content))
            return Result.Failure<TaskComment>(TaskCommentErrors.ContentRequired);

        return Result.Success(new TaskComment
        {
            Id = Guid.NewGuid(),
            Content = content,
            AuthorId = authorId,
            CreatedAt = DateTime.UtcNow
        });
    }
}
