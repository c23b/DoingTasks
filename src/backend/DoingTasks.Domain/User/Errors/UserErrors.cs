using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Domain.User;

// Domain/Users/UserErrors.cs
public static class UserErrors
{
    public static readonly Error FullNameRequired =
        Error.Validation("User.FullNameRequired", "Full name is required");

    public static readonly Error EmailRequired =
        Error.Validation("User.EmailRequired", "Email is required");

    public static readonly Error NotFound =
        Error.NotFound("User.NotFound", "User was not found");

    public static readonly Error IdentityError =
        Error.Problem("User.IdentityError", "An error occurred while creating the user");
}
