using Refit;

namespace DoingTasks.Infrastructure.ExternalServices.Microsoft;

[Headers("Accept: application/json")]
public interface IMicrosoftTokenApi
{
    [Post("/v2.0/token")]
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    Task<ExchangeCodeResponse> ExchangeCodeAsync([Body(BodySerializationMethod.UrlEncoded)] ExchangeCodeRequest request);
}
