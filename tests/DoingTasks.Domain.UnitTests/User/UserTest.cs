using DoingTasks.Domain.User;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.UnitTests;

/// <summary>
/// Unit tests for the <see cref="User.User"/> aggregate root.
/// </summary>
/// <remarks>
/// Tests cover user creation and update operations, validating both success and failure scenarios
/// with comprehensive error handling validation.
/// </remarks>
[Collection(nameof(UserCollection))]
public class UserTest
{
    private readonly UserTestFixture _userTestFixture;

    public UserTest(UserTestFixture userTestFixture)
    {
        _userTestFixture = userTestFixture;
    }

    /// <summary>
    /// Tests successful creation of a user with valid parameters.
    /// </summary>
    /// <remarks>
    /// Verifies that when creating a user with valid full name, email, nickname, and birth date,
    /// the operation succeeds and returns a user object with all properties correctly set.
    /// </remarks>
    [Fact(DisplayName = "User  - Create Success")]
    public void User_Create_Success()
    {
        var birthDate = new DateOnly(1990, 01, 01);
        var resultUser = User.User.Create("Person Silva", "person@test.com", "Person", birthDate);

        Assert.NotNull(resultUser);
        Assert.True(resultUser.IsSuccess);
        Assert.Equal("Person Silva", resultUser.Value.FullName);
        Assert.Equal("person@test.com", resultUser.Value.Email);
        Assert.Equal("Person", resultUser.Value.Nickname.Value);
        Assert.Equal(birthDate, resultUser.Value.BirthDate);
    }

    /// <summary>
    /// Tests user creation failure when full name is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that when creating a user with an empty full name, the operation fails
    /// and returns the appropriate <see cref="UserErrors.FullNameRequired"/> error.
    /// </remarks>
    [Fact(DisplayName = "User  - Create Error FullNameRequired")]
    public void User_Create_Error_FullNameRequired()
    {
        var resultUser = User.User.Create(string.Empty, 
                                          "person@test.com", 
                                          "Person", 
                                          new DateOnly(1990, 01, 01));

        Assert.NotNull(resultUser);
        Assert.True(resultUser.IsFailure);
        Assert.NotNull(resultUser.Error);        
        Assert.Equal(UserErrors.FullNameRequired.Code, resultUser.Error.Code);
        Assert.Equal(UserErrors.FullNameRequired.Description, resultUser.Error.Description);
    }

    /// <summary>
    /// Tests user creation failure when email is invalid or empty.
    /// </summary>
    /// <remarks>
    /// Verifies that when creating a user with an invalid email format or empty email,
    /// the operation fails and returns the appropriate <see cref="UserErrors.EmailInvalid"/> error.
    /// </remarks>
    /// <param name="email">The invalid email to test (non-email format or empty string).</param>
    [Theory(DisplayName = "User  - Create Error EmailInvalid")]
    [InlineData("invalid-email")]
    [InlineData("")]
    public void User_Create_Error_EmailInvalid(string email)
    {
        var resultUser = User.User.Create("Person Silva", email, "Person", new DateOnly(1990, 01, 01));

        Assert.NotNull(resultUser);
        Assert.True(resultUser.IsFailure);
        Assert.NotNull(resultUser.Error);
        Assert.Equal(UserErrors.EmailInvalid.Code, resultUser.Error.Code);
        Assert.Equal(UserErrors.EmailInvalid.Description, resultUser.Error.Description);
    }

    /// <summary>
    /// Tests user creation failure when birth date is invalid.
    /// </summary>
    /// <remarks>
    /// Verifies that when creating a user with a birth date in the future (last year or later),
    /// the operation fails and returns the appropriate <see cref="UserErrors.BirthDateInvalid"/> error.
    /// </remarks>
    [Fact(DisplayName = "User  - Create Error BirthDateInvalid")]
    public void User_Create_Error_BirthDateInvalid()
    {
        var resultUser = User.User.Create("Person Silva", 
                                          "person@test.com", 
                                          "Person", 
                                          new DateOnly((DateTime.UtcNow.Year - 1), 01, 01));

        Assert.NotNull(resultUser);
        Assert.True(resultUser.IsFailure);
        Assert.NotNull(resultUser.Error);
        Assert.Equal(UserErrors.BirthDateInvalid.Code, resultUser.Error.Code);
        Assert.Equal(UserErrors.BirthDateInvalid.Description, resultUser.Error.Description);
    }

    /// <summary>
    /// Tests user creation failure when nickname is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that when creating a user with an empty nickname, the operation fails
    /// and returns the appropriate <see cref="NicknameErrors.Required"/> error.
    /// </remarks>
    [Fact(DisplayName = "User  - Create Error NicknameRequired")]
    public void User_Create_Error_NicknameRequired()
    {
        var resultUser = User.User.Create("Person Silva", "person@test.com", "", new DateOnly(1990, 01, 01));

        Assert.NotNull(resultUser);
        Assert.True(resultUser.IsFailure);
        Assert.NotNull(resultUser.Error);
        Assert.Equal(NicknameErrors.Required.Code, resultUser.Error.Code);
        Assert.Equal(NicknameErrors.Required.Description, resultUser.Error.Description);
    }

    /// <summary>
    /// Tests user creation failure when nickname exceeds maximum length.
    /// </summary>
    /// <remarks>
    /// Verifies that when creating a user with a nickname that exceeds the maximum allowed length,
    /// the operation fails and returns the appropriate <see cref="NicknameErrors.TooLong"/> error.
    /// </remarks>
    [Fact(DisplayName = "User  - Create Error NicknameTooLong")]
    public void User_Create_Error_NicknameTooLong()
    {
        var resultUser = User.User.Create("Person Silva", 
                                          "person@test.com",
                                          "Error Nickname TooLong Error Nickname TooLong Error Nickname TooLong", 
                                          new DateOnly(1990, 01, 01));

        Assert.NotNull(resultUser);
        Assert.True(resultUser.IsFailure);
        Assert.NotNull(resultUser.Error);
        Assert.Equal(NicknameErrors.TooLong.Code, resultUser.Error.Code);
        Assert.Equal(NicknameErrors.TooLong.Description, resultUser.Error.Description);
    }

    /// <summary>
    /// Tests successful update of user properties.
    /// </summary>
    /// <remarks>
    /// Verifies that when updating a user with valid full name, nickname, and birth date,
    /// the operation succeeds and the user object is updated with the new values.
    /// </remarks>
    [Fact(DisplayName = "User  - Update Success")]
    public void User_Update_Success()
    {
        var user = _userTestFixture.GenerateUser();

        var birthDate = new DateOnly(1992, 02, 02);
        var resultUser = user.Update("Person Updated", "Updated", birthDate);

        Assert.NotNull(resultUser);
        Assert.True(resultUser.IsSuccess);
        Assert.Equal("Person Updated", user.FullName);
        Assert.Equal("Updated", user.Nickname.Value);
        Assert.Equal(birthDate, user.BirthDate);
    }

    /// <summary>
    /// Tests user update failure when full name is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that when updating a user with an empty full name, the operation fails
    /// and returns the appropriate <see cref="UserErrors.FullNameRequired"/> error.
    /// </remarks>
    [Fact(DisplayName = "User  - Update Error FullNameRequired")]
    public void User_Update_Error_FullNameRequired()
    {
        var user = _userTestFixture.GenerateUser();

        var birthDate = new DateOnly(1992, 02, 02);
        var resultUser = user.Update("", "Updated", birthDate);

        Assert.NotNull(resultUser);
        Assert.True(resultUser.IsFailure);
        Assert.NotNull(resultUser.Error);
        Assert.Equal(UserErrors.FullNameRequired.Code, resultUser.Error.Code);
        Assert.Equal(UserErrors.FullNameRequired.Description, resultUser.Error.Description);
    }

    /// <summary>
    /// Tests user update failure when nickname is empty.
    /// </summary>
    /// <remarks>
    /// Verifies that when updating a user with an empty nickname, the operation fails
    /// and returns the appropriate <see cref="NicknameErrors.Required"/> error.
    /// </remarks>
    [Fact(DisplayName = "User  - Update Error NicknameRequired")]
    public void User_Update_Error_NicknameRequired()
    {
        var user = _userTestFixture.GenerateUser();

        var birthDate = new DateOnly(1992, 02, 02);
        var resultUser = user.Update("Updated", "", birthDate);

        Assert.NotNull(resultUser);
        Assert.True(resultUser.IsFailure);
        Assert.NotNull(resultUser.Error);
        Assert.Equal(NicknameErrors.Required.Code, resultUser.Error.Code);
        Assert.Equal(NicknameErrors.Required.Description, resultUser.Error.Description);
    }

    /// <summary>
    /// Tests user update failure when nickname exceeds maximum length.
    /// </summary>
    /// <remarks>
    /// Verifies that when updating a user with a nickname that exceeds the maximum allowed length,
    /// the operation fails and returns the appropriate <see cref="NicknameErrors.TooLong"/> error.
    /// </remarks>
    [Fact(DisplayName = "User  - Update Error NicknameTooLong")]
    public void User_Update_Error_NicknameTooLong()
    {
        var user = _userTestFixture.GenerateUser();

        var birthDate = new DateOnly(1992, 02, 02);
        var resultUser = user.Update("Updated",
                                     "Update Error NicknameTooLong Update Error NicknameTooLong", 
                                     birthDate);

        Assert.NotNull(resultUser);
        Assert.True(resultUser.IsFailure);
        Assert.NotNull(resultUser.Error);
        Assert.Equal(NicknameErrors.TooLong.Code, resultUser.Error.Code);
        Assert.Equal(NicknameErrors.TooLong.Description, resultUser.Error.Description);
    }

    /// <summary>
    /// Tests user update failure when birth date is invalid.
    /// </summary>
    /// <remarks>
    /// Verifies that when updating a user with a birth date in the future (last year or later),
    /// the operation fails and returns the appropriate <see cref="UserErrors.BirthDateInvalid"/> error.
    /// </remarks>
    [Fact(DisplayName = "User  - Update Error BirthDateInvalid")]
    public void User_Update_Error_BirthDateInvalid()
    {
        var user = _userTestFixture.GenerateUser();

        var birthDate = new DateOnly((DateTime.UtcNow.Year - 1), 01, 01);
        var resultUser = user.Update("Person Updated",
                                     "Update",
                                     birthDate);

        Assert.NotNull(resultUser);
        Assert.True(resultUser.IsFailure);
        Assert.NotNull(resultUser.Error);
        Assert.Equal(UserErrors.BirthDateInvalid.Code, resultUser.Error.Code);
        Assert.Equal(UserErrors.BirthDateInvalid.Description, resultUser.Error.Description);
    }
}
