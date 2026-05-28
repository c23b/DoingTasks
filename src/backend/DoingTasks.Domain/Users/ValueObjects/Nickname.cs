using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.Users;

public sealed record Nickname
{
    public string Value { get; }

    private Nickname(string value) => Value = value;

    public static Result<Nickname> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Nickname>(NicknameErrors.Required);

        if (value.Length > 30)
            return Result.Failure<Nickname>(NicknameErrors.TooLong);

        return Result.Success(new Nickname(value.Trim()));
    }
}
