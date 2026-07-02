using Microsoft.AspNetCore.Identity;

namespace DoingTasks.Infrastructure.Authentication.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public Guid DomainUserId { get; set; }
    public bool IsExternalLogin { get; set; }
}
