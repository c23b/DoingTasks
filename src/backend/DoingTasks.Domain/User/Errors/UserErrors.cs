using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.User;

public static class UserErrors
{
    public static readonly Error FullNameRequired =
        Error.Validation("User.FullNameRequired", "Full name is required");

    public static readonly Error EmailInvalid =
        Error.Validation("User.EmailRequired", "Email is invalid");

    public static readonly Error BirthDateInvalid =
        Error.Validation("User.BirthDateInvalid", "The user must have 18 year or more");

    public static readonly Error NotFound =
        Error.NotFound("User.NotFound", "User was not found");

    public static readonly Error IdentityError =
        Error.Problem("User.IdentityError", "An error occurred while creating the user");
}
