using Refit;

namespace DoingTasks.Infrastructure.ExternalServices.Google;

[Headers("Accept: application/json")]
public interface IGoogleUserInfoApi
{
    [Get("/v3/userinfo")]
    Task<GetUserInfoResponse> GetUserInfoAsync([Header("Authorization")] string authorization);
}
