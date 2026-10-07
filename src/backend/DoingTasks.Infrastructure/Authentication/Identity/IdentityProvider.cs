using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.DTOs.Authentication;
using DoingTasks.Application.Errors;
using DoingTasks.SharedKernel.Results;
using Microsoft.AspNetCore.Identity;

namespace DoingTasks.Infrastructure.Authentication.Identity;

internal sealed class IdentityProvider(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ITokenProvider tokenProvider) : IIdentityProvider
{
    public async Task<Result<RegisterResult>> RegisterAsync(
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
            return Result.Failure<RegisterResult>(AuthenticationErrors.IdentityError);

        var token = tokenProvider.GenerateToken(appUser.Id, email, domainUserId);
        return Result.Success(new RegisterResult(appUser.Id, token));
    }

    public async Task<Result<AuthenticationResponse>> LoginAsync(
        string email,
        string password)
    {
        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser is null)
            return Result.Failure<AuthenticationResponse>(AuthenticationErrors.InvalidCredentials);

        var result = await signInManager.PasswordSignInAsync(
            appUser,
            password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Result.Failure<AuthenticationResponse>(AuthenticationErrors.AccountLockedOut);

        if (!result.Succeeded)
            return Result.Failure<AuthenticationResponse>(AuthenticationErrors.InvalidCredentials);

        var token = tokenProvider.GenerateToken(appUser.Id, email, appUser.DomainUserId);
        return Result.Success(new AuthenticationResponse(token));
    }

    public async Task<Result<AuthenticationResponse>> RefreshTokenAsync(string identityId)
    {
        var appUser = await userManager.FindByIdAsync(identityId);
        if (appUser is null)
            return Result.Failure<AuthenticationResponse>(AuthenticationErrors.NotFound);

        var token = tokenProvider.GenerateToken(appUser.Id, appUser.Email!, appUser.DomainUserId);
        return Result.Success(new AuthenticationResponse(token));
    }

    public async Task<Result> ConfirmEmailAsync(
        string userId,
        string token)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> ResendConfirmationEmailAsync(
        string email)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> ForgotPasswordAsync(
        string email)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> ResetPasswordAsync(
        string email,
        string token,
        string newPassword)
    {
        throw new NotImplementedException();
    }

    public async Task<Result> DeleteAsync(string identityId)
    {
        var appUser = await userManager.FindByIdAsync(identityId);
        if (appUser is null)
            return Result.Failure(AuthenticationErrors.NotFound);

        var result = await userManager.DeleteAsync(appUser);
        if (!result.Succeeded)
            return Result.Failure(AuthenticationErrors.IdentityError);

        return Result.Success();
    }

    public async Task<Result<RegisterResult>> GoogleRegisterAsync(
    string googleId,
    string email,
    string name,
    Guid domainUserId) =>
    await ExternalRegisterAsync(googleId, "Google", email, domainUserId);

    public async Task<Result<RegisterResult>> MicrosoftRegisterAsync(
        string microsoftId,
        string email,
        string name,
        Guid domainUserId) =>
        await ExternalRegisterAsync(microsoftId, "Microsoft", email, domainUserId);

    private async Task<Result<RegisterResult>> ExternalRegisterAsync(
        string providerId,
        string providerName,
        string email,
        Guid domainUserId)
    {
        var appUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DomainUserId = domainUserId,
            IsExternalLogin = true,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(appUser);
        if (!createResult.Succeeded)
            return Result.Failure<RegisterResult>(AuthenticationErrors.IdentityError);

        var loginResult = await userManager.AddLoginAsync(appUser, new UserLoginInfo(
            providerName,
            providerId,
            providerName));

        if (!loginResult.Succeeded)
            return Result.Failure<RegisterResult>(AuthenticationErrors.IdentityError);

        var token = tokenProvider.GenerateToken(appUser.Id, email, domainUserId);
        return Result.Success(new RegisterResult(appUser.Id, token));
    }

    private async Task<Result<AuthenticationResponse>> ExternalLoginAsync(string provider, string providerId)
    {
        var result = await signInManager.ExternalLoginSignInAsync(
            provider,
            providerId,
            isPersistent: false,
            bypassTwoFactor: true);

        if (result.IsLockedOut)
            return Result.Failure<AuthenticationResponse>(AuthenticationErrors.AccountLockedOut);

        if (!result.Succeeded)
            return Result.Failure<AuthenticationResponse>(AuthenticationErrors.InvalidCredentials);

        var appUser = await userManager.FindByLoginAsync(provider, providerId);
        if (appUser is null)
            return Result.Failure<AuthenticationResponse>(AuthenticationErrors.InvalidCredentials);

        var token = tokenProvider.GenerateToken(appUser.Id, appUser.Email!, appUser.DomainUserId);
        return Result.Success(new AuthenticationResponse(token));
    }

    public async Task<Result<AuthenticationResponse>> GoogleLoginAsync(string googleId) =>
        await ExternalLoginAsync("Google", googleId);

    public async Task<Result<AuthenticationResponse>> MicrosoftLoginAsync(string microsoftId) =>
        await ExternalLoginAsync("Microsoft", microsoftId);
}
