using DoingTasks.Domain.User;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.UnitTests;

[Collection(nameof(UserCollection))]
public class UserTest
{
    private readonly UserTestFixture _userTestFixture;

    public UserTest(UserTestFixture userTestFixture)
    {
        _userTestFixture = userTestFixture;
    }

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

    [Theory(DisplayName = "User  - Create Error EmailInvalid")]
    [InlineData("fasdfasdfaasdf")]
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
