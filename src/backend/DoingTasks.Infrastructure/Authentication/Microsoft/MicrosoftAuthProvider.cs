using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.DTOs.Microsoft;
using DoingTasks.Application.Errors;
using DoingTasks.Infrastructure.Authentication.Google;
using DoingTasks.Infrastructure.Authentication.Helpers;
using DoingTasks.Infrastructure.Exceptions;
using DoingTasks.Infrastructure.ExternalServices.Common;
using DoingTasks.Infrastructure.ExternalServices.Microsoft;
using DoingTasks.SharedKernel.Results;
using Microsoft.Extensions.Options;
using Refit;

namespace DoingTasks.Infrastructure.Authentication.Microsoft;

internal sealed class MicrosoftAuthProvider(
    IMicrosoftTokenApi microsoftTokenApi,
    IMicrosoftUserInfoApi microsoftUserInfoApi,
    IOptions<MicrosoftAuthSettings> microsoftAuthSettings) : IMicrosoftAuthProvider
{
    public async Task<Result<MicrosoftUserInfo>> AuthenticateAsync(
        string code,
        string redirectUri,
        string codeVerifier,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result.Failure<MicrosoftUserInfo>(AuthenticationErrors.InvalidCode);

        if (string.IsNullOrWhiteSpace(redirectUri))
            return Result.Failure<MicrosoftUserInfo>(AuthenticationErrors.InvalidRedirectUri);

        if (string.IsNullOrWhiteSpace(codeVerifier))
            return Result.Failure<MicrosoftUserInfo>(AuthenticationErrors.InvalidCodeVerifier);

        var accessTokenResult = await ExchangeCodeAsync(code, redirectUri, codeVerifier, ct);
        if (accessTokenResult.IsFailure)
            return Result.Failure<MicrosoftUserInfo>(accessTokenResult.Error);

        return await GetUserInfoAsync(accessTokenResult.Value, ct);
    }

    private async Task<Result<string>> ExchangeCodeAsync(
        string code,
        string redirectUri,
        string codeVerifier,
        CancellationToken ct = default)
    {
        try
        {
            var request = new ExchangeCodeRequest
            {
                ClientId = microsoftAuthSettings.Value.ClientId,
                ClientSecret = microsoftAuthSettings.Value.Secret,
                Code = code,
                RedirectUri = redirectUri,
                CodeVerifier = codeVerifier
            };

            var response = await microsoftTokenApi.ExchangeCodeAsync(request);
            if (!response.IsValid)
                return Result.Failure<string>(AuthenticationErrors.InvalidMicrosoftToken);

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

    private async Task<Result<MicrosoftUserInfo>> GetUserInfoAsync(
        string accessToken,
        CancellationToken ct = default)
    {
        try
        {
            var response = await microsoftUserInfoApi.GetUserInfoAsync(accessToken);
            if (!response.IsValid)
                return Result.Failure<MicrosoftUserInfo>(AuthenticationErrors.InvalidMicrosoftToken);

            return Result.Success(new MicrosoftUserInfo
            {
                MicrosoftId = response.Id,
                Email = response.Email,
                Name = response.DisplayName
            });
        }
        catch (ApiException apiEx)
        {
            // Se o provedor retornou JSON OAuth → helper classifica
            if (apiEx.HasContent)
            {
                var error = await apiEx.GetContentAsAsync<OAuthErrorResponse>();
                var classified = OAuthErrorHelper.Classify("userinfo", error);
                return Result.Failure<MicrosoftUserInfo>(classified);
            }
            // Sem JSON → erro de infraestrutura
            return Result.Failure<MicrosoftUserInfo>(AuthenticationErrors.ProviderUnavailable);
        }
        catch (Exception ex)
        {
            return Result.Failure<MicrosoftUserInfo>(MapInfrastructureException.MapInfrastructureError(ex));
        }
    }
}