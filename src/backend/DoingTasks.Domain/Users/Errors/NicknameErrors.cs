using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Users;

public static class NicknameErrors
{
    public static readonly Error Required =
        Error.Validation("Nickname.Required", "Nickname is required");

    public static readonly Error TooLong =
        Error.Validation("Nickname.TooLong", "Nickname cannot exceed 30 characters");
}
