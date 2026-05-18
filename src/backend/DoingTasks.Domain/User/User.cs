using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;
using DoingTasks.SharedKernel.Util;

namespace DoingTasks.Domain.User;

public sealed class User : AggregateRoot
{
    public string FullName { get; private set; }
    public Nickname Nickname { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string Email { get; private set; }

    private User() { }

    public static Result<User> Create(string fullName, string email, string nickname, DateOnly birthDate)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return Result.Failure<User>(UserErrors.FullNameRequired);

        if (string.IsNullOrWhiteSpace(email) || !EmailRules.Verify(email))
            return Result.Failure<User>(UserErrors.EmailInvalid);

        var nicknameResult = Nickname.Create(nickname);
        if (nicknameResult.IsFailure)
            return Result.Failure<User>(nicknameResult.Error);

        if((DateTime.UtcNow.Year - birthDate.Year) < 18)
            return Result.Failure<User>(UserErrors.BirthDateInvalid);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Nickname = nicknameResult.Value,
            BirthDate = birthDate,
            Email = email
        };

        //user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
        return Result.Success(user);
    }

    public Result Update(string fullName, string nickname, DateOnly birthDate)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return Result.Failure<User>(UserErrors.FullNameRequired);
              
        var nicknameResult = Nickname.Create(nickname);
        if (nicknameResult.IsFailure)
            return Result.Failure<User>(nicknameResult.Error);

        if ((DateTime.UtcNow.Year - birthDate.Year) < 18)
            return Result.Failure<User>(UserErrors.BirthDateInvalid);

        FullName = fullName;
        BirthDate = birthDate;
        Nickname = nicknameResult.Value;

        //user.RaiseDomainEvent(new UserUpdatedDomainEvent(user.Id));
        return Result.Success();
    }
}
