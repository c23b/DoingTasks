using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.User;

public sealed class User : AggregateRoot
{
    public string FullName { get; private set; }
    public Nickname Nickname { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string Email { get; private set; }

    private User() { }

    public static Result<User> Create(string fullName, string nickname, DateOnly birthDate, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return Result.Failure<User>(UserErrors.FullNameRequired);

        if (string.IsNullOrWhiteSpace(email))
            return Result.Failure<User>(UserErrors.EmailRequired);

        var nicknameResult = Nickname.Create(nickname);
        if (nicknameResult.IsFailure)
            return Result.Failure<User>(nicknameResult.Error);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Nickname = nicknameResult.Value,
            BirthDate = birthDate,
            Email = email
        };

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
        return Result.Success(user);
    }

    public Result UpdateNickname(string nickname)
    {
        var nicknameResult = Nickname.Create(nickname);
        if (nicknameResult.IsFailure)
            return Result.Failure(nicknameResult.Error);

        Nickname = nicknameResult.Value;
        return Result.Success();
    }
}
