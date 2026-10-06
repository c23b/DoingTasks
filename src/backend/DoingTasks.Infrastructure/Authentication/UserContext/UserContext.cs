using DoingTasks.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace DoingTasks.Infrastructure.Authentication.UserContext;
internal sealed class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid UserId => 
        Guid.Parse(httpContextAccessor.HttpContext!.User.FindFirstValue("domain_user_id")!);

    public string IdentityId => 
        httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
