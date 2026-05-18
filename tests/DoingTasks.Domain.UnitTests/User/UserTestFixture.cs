namespace DoingTasks.Domain.UnitTests;

[CollectionDefinition(nameof(UserCollection))]
public class UserCollection : ICollectionFixture<UserTestFixture> { }

public class UserTestFixture : IDisposable
{

    public UserTestFixture()
    {

    }
    /// <summary>
    /// Returns a valid user for testing purposes. Person Slva - person@test.com - Pessoa - 01/01/1990"
    /// </summary>
    /// <returns></returns>
    public User.User GenerateUser()
    {
        return User.User.Create("Person Slva", "person@test.com", "Pessoa", new DateOnly(1990, 01, 01)).Value;
    }

    public User.User GenerateUser(string fullName, string email, string nickname, DateOnly birthDate)
    {
        return User.User.Create(fullName, email, nickname, birthDate).Value;
    }


    public void Dispose()
    {

    }
}

