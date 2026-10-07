using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.DTOs.Google;
using DoingTasks.Application.Errors;
using DoingTasks.Infrastructure.Authentication.Helpers;
using DoingTasks.Infrastructure.Exceptions;
using DoingTasks.Infrastructure.ExternalServices.Common;
using DoingTasks.Infrastructure.ExternalServices.Google;
using DoingTasks.SharedKernel.Results;
using Microsoft.Extensions.Options;
using Refit;

namespace DoingTasks.Infrastructure.Authentication.Google;

internal sealed class GoogleAuthProvider(
    IGoogleTokenApi googleTokenApi,
    IGoogleUserInfoApi googleUserInfoApi,
    IOptions<GoogleAuthSettings> googleAuthSettings)
    : IGoogleAuthProvider
{
    public async Task<Result<GoogleUserInfo>> AuthenticateAsync(
        string code,
        string redirectUri,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<GoogleUserInfo>(AuthenticationErrors.InvalidCode);

        if (string.IsNullOrWhiteSpace(redirectUri))
            return Result.Failure<GoogleUserInfo>(AuthenticationErrors.InvalidRedirectUri);

        var accessTokenResult = await ExchangeCodeAsync(code, redirectUri, ct);

        if (accessTokenResult.IsFailure)
            return Result.Failure<GoogleUserInfo>(accessTokenResult.Error);

        return await GetUserInfoAsync(accessTokenResult.Value, ct);
    }

    private async Task<Result<string>> ExchangeCodeAsync(
        string code,
        string redirectUri,
        CancellationToken ct = default)
    {
        try
        {
            var request = new ExchangeCodeRequest
            {
                Code = code,
                ClientId = googleAuthSettings.Value.ClientId,
                ClientSecret = googleAuthSettings.Value.Secret,
                RedirectUri = redirectUri,
            };

            var response = await googleTokenApi.ExchangeCodeAsync(request);

            if (!response.IsValid)
                return Result.Failure<string>(AuthenticationErrors.InvalidGoogleToken);

            return Result.Success(response.AccessToken);

        }
        catch (ApiException apiEx)
        {
            // Se o provedor retornou JSON OAuth → helper classifica
            if (apiEx.HasContent)
            {
                var error = await apiEx.GetContentAsAsync<OAuthErrorResponse>();
                var classified = OAuthErrorHelper.Classify("token", error);
                return Result.Failure<string>(classified);
            }
            // Sem JSON → erro de infraestrutura
            return Result.Failure<string>(AuthenticationErrors.ProviderUnavailable);
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(MapInfrastructureException.MapInfrastructureError(ex));
        }
    }

    private async Task<Result<GoogleUserInfo>> GetUserInfoAsync(
        string authorization,
        CancellationToken ct = default)
    {
        try
        {
            var response = await googleUserInfoApi.GetUserInfoAsync($"Bearer {authorization}");

            if (!response.IsValid)
                return Result.Failure<GoogleUserInfo>(AuthenticationErrors.InvalidGoogleToken);

            return Result.Success(new GoogleUserInfo
            {
                GoogleId = response.Id,
                Email = response.Email,
                Name = response.Name
            });
        }
        catch (ApiException apiEx)
        {
            // Se o provedor retornou JSON OAuth → helper classifica
            if (apiEx.HasContent)
            {
                var error = await apiEx.GetContentAsAsync<OAuthErrorResponse>();
                var classified = OAuthErrorHelper.Classify("userinfo", error);
                return Result.Failure<GoogleUserInfo>(classified);
            }
            // Sem JSON → erro de infraestrutura
            return Result.Failure<GoogleUserInfo>(AuthenticationErrors.ProviderUnavailable);
        }
        catch (Exception ex)
        {
            return Result.Failure<GoogleUserInfo>(MapInfrastructureException.MapInfrastructureError(ex));
        }
    }
}
