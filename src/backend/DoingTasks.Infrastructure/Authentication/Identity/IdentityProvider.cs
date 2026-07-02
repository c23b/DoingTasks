using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.Errors;
using DoingTasks.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;

namespace DoingTasks.Infrastructure.Authentication.Identity;

internal sealed class IdentityProvider(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenProvider tokenProvider) : IIdentityProvider
{
    public async Task<Result<string>> RegisterAsync(
        string email,
        string password,
        Guid domainUserId)
    {
        var appUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DomainUserId = domainUserId,
            IsExternalLogin = false
        };

        var result = await userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
            return Result.Failure<string>(AuthenticationErrors.IdentityError);

        var token = tokenProvider.GenerateToken(appUser.Id, email, domainUserId);
        return Result.Success(token);
    }

    public async Task<Result<string>> LoginAsync(string email, string password)
    {
        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser is null)
            return Result.Failure<string>(AuthenticationErrors.InvalidCredentials);

        var result = await signInManager.PasswordSignInAsync(
            appUser,
            password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Result.Failure<string>(AuthenticationErrors.AccountLockedOut);

        if (!result.Succeeded)
            return Result.Failure<string>(AuthenticationErrors.InvalidCredentials);

        var token = tokenProvider.GenerateToken(appUser.Id, email, appUser.DomainUserId);
        return Result.Success(token);
    }

    public async Task<Result<string>> RefreshTokenAsync(string userId, string refreshToken)
    {
        var appUser = await userManager.FindByIdAsync(userId);
        if (appUser is null)
            return Result.Failure<string>(AuthenticationErrors.NotFound);

        var isValid = await userManager.VerifyUserTokenAsync(
            appUser,
            TokenOptions.DefaultProvider,
            "RefreshToken",
            refreshToken);

        if (!isValid)
            return Result.Failure<string>(AuthenticationErrors.InvalidRefreshToken);

        var token = tokenProvider.GenerateToken(appUser.Id, appUser.Email!, appUser.DomainUserId);
        return Result.Success(token);
    }

    public async Task<Result> ConfirmEmailAsync(string userId, string token)
    {
        throw new NotImplementedException();
        var appUser = await userManager.FindByIdAsync(userId);
        if (appUser is null)
            return Result.Failure(AuthenticationErrors.NotFound);

        var result = await userManager.ConfirmEmailAsync(appUser, token);
        if (!result.Succeeded)
            return Result.Failure(AuthenticationErrors.InvalidEmailConfirmationToken);

        return Result.Success();
    }

    public async Task<Result> ResendConfirmationEmailAsync(string email)
    {
        throw new NotImplementedException();
        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser is null)
            return Result.Failure(AuthenticationErrors.NotFound);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(appUser);
        // TODO: enviar email com token
        return Result.Success();
    }

    public async Task<Result> ForgotPasswordAsync(string email)
    {
        throw new NotImplementedException();
        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser is null)
            return Result.Success(); // não revela se email existe

        var token = await userManager.GeneratePasswordResetTokenAsync(appUser);
        // TODO: enviar email com token
        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword)
    {
        throw new NotImplementedException();
        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser is null)
            return Result.Failure(AuthenticationErrors.NotFound);

        var result = await userManager.ResetPasswordAsync(appUser, token, newPassword);
        if (!result.Succeeded)
            return Result.Failure(AuthenticationErrors.InvalidPasswordResetToken);

        return Result.Success();
    }
}
