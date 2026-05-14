using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Task;

public static class TaskCommentErrors
{
    public static readonly Error ContentRequired =
        Error.Validation("TaskComment.ContentRequired", "Comment content is required");

    public static readonly Error NotFound =
        Error.NotFound("TaskComment.NotFound", "Comment was not found");
}