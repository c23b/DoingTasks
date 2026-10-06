
namespace DoingTasks.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string GenerateToken(string userId, string email, Guid domainUserId);
}
