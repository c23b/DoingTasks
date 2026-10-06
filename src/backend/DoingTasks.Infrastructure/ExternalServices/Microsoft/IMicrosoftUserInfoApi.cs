using Refit;

namespace DoingTasks.Infrastructure.ExternalServices.Microsoft;

[Headers("Accept: application/json")]
public interface IMicrosoftUserInfoApi
{
    [Get("/v1.0/me")]
    Task<MicrosoftApiResponse> GetUserInfoAsync([Authorize("Bearer")] string accessToken);
}
