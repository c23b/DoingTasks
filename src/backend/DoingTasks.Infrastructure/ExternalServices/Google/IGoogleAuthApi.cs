using Refit;

namespace DoingTasks.Infrastructure.ExternalServices.Google;

[Headers("Accept: application/json")]
public interface IGoogleAuthApi
{
    [Get("/tokeninfo")]
    Task<GoogleApiResponse> ValidateTokenAsync([Query] string id_token);
}
