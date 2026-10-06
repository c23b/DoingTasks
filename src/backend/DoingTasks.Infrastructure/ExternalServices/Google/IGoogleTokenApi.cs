using DoingTasks.Application.DTOs.Google;
using Refit;

namespace DoingTasks.Infrastructure.ExternalServices.Google;

[Headers("Accept: application/json")]
public interface IGoogleTokenApi
{
    [Post("/token")]
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    Task<ExchangeCodeResponse> ExchangeCodeAsync([Body(BodySerializationMethod.UrlEncoded)] ExchangeCodeRequest request);
    
}
