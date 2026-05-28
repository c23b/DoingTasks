using DoingTasks.SharedKernel.Domain;
using DoingTasks.SharedKernel.Results;
using DoingTasks.SharedKernel.Util;

namespace DoingTasks.Domain.Users;

/// <summary>
/// Represents a user aggregate root in the domain.
/// </summary>
/// <remarks>
/// The User aggregate manages user identity, profile information, and business rules validation.
/// Users must be at least 18 years old and have valid email addresses. This class implements
/// the Result pattern for operation outcomes to support functional error handling.
/// </remarks>
public sealed class User : AggregateRoot
{
    /// <summary>
    /// Gets the full name of the user.
    /// </summary>
    /// <value>
    /// A non-empty string containing the user's full name.
    /// </value>
    public string FullName { get; private set; }

    /// <summary>
    /// Gets the user's nickname (username).
    /// </summary>
    /// <value>
    /// A <see cref="Nickname"/> value object containing the user's nickname with enforced length constraints.
    /// </value>
    public Nickname Nickname { get; private set; }

    /// <summary>
    /// Gets the user's birth date.
    /// </summary>
    /// <value>
    /// A <see cref="DateOnly"/> representing the user's date of birth. User must be at least 18 years old.
    /// </value>
    public DateOnly BirthDate { get; private set; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    /// <value>
    /// A non-empty string containing the user's email address in valid format.
    /// </value>
    public string Email { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor is private and used only by the ORM or internal factory methods.
    /// Instances should be created using the <see cref="Create"/> factory method.
    /// </remarks>
    private User() { }

    /// <summary>
    /// Creates a new user with the specified parameters.
    /// </summary>
    /// <remarks>
    /// This factory method validates all input parameters according to domain rules:
    /// - Full name is required and cannot be empty or whitespace.
    /// - Email must be provided and match a valid email format.
    /// - Nickname must be valid according to <see cref="Nickname"/> value object rules.
    /// - User must be at least 18 years old as of the current UTC date.
    /// </remarks>
    /// <param name="fullName">The full name of the user. Cannot be null, empty, or whitespace.</param>
    /// <param name="email">The email address of the user. Must be a valid email format.</param>
    /// <param name="nickname">The nickname/username of the user. Must meet nickname validation rules.</param>
    /// <param name="birthDate">The birth date of the user. User must be at least 18 years old.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the newly created <see cref="User"/> on success,
    /// or an error containing <see cref="UserErrors"/> on failure.
    /// </returns>
    /// <exception cref="UserErrors.FullNameRequired">
    /// Thrown when full name is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="UserErrors.EmailInvalid">
    /// Thrown when email is null, empty, or does not match a valid email format.
    /// </exception>
    /// <exception cref="NicknameErrors">
    /// Thrown when nickname validation fails (required or too long).
    /// </exception>
    /// <exception cref="UserErrors.BirthDateInvalid">
    /// Thrown when the user is not at least 18 years old.
    /// </exception>
    public static Result<User> Create(
        string fullName, 
        string email, 
        string nickname, 
        DateOnly birthDate)
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
            FullName = fullName,
            Nickname = nicknameResult.Value,
            BirthDate = birthDate,
            Email = email,
        };

        user.RaiseDomainEvent(new UserCreatedDomainEvent(user.Id));
        return Result.Success(user);
    }

    /// <summary>
    /// Updates the user's profile information.
    /// </summary>
    /// <remarks>
    /// This method updates the user's full name, nickname, and birth date with the same validation rules
    /// as the <see cref="Create"/> factory method. The email address cannot be updated through this method.
    /// All validation rules are re-applied before any changes are persisted.
    /// </remarks>
    /// <param name="fullName">The new full name for the user. Cannot be null, empty, or whitespace.</param>
    /// <param name="nickname">The new nickname for the user. Must meet nickname validation rules.</param>
    /// <param name="birthDate">The new birth date for the user. User must be at least 18 years old.</param>
    /// <returns>
    /// A <see cref="Result"/> indicating success if all validations pass and the update is applied,
    /// or an error containing <see cref="UserErrors"/> or <see cref="NicknameErrors"/> on failure.
    /// </returns>
    /// <exception cref="UserErrors.FullNameRequired">
    /// Thrown when full name is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="NicknameErrors">
    /// Thrown when nickname validation fails (required or too long).
    /// </exception>
    /// <exception cref="UserErrors.BirthDateInvalid">
    /// Thrown when the user would not be at least 18 years old with the new birth date.
    /// </exception>
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

        RaiseDomainEvent(new UserUpdatedDomainEvent(this.Id));
        return Result.Success();
    }
}
