using DoingTasks.Api.Extensions;
using DoingTasks.Api.Infrastructure;
using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;
using DoingTasks.Application.Features.Authentication.Commands.RefreshToken;
namespace DoingTasks.Api.Endpoints.Authentication;
internal sealed class RefreshToken : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/refresh", async (
            ICommandHandler<RefreshTokenCommand, AuthenticationResponse> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new RefreshTokenCommand(), ct);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Authentication)
        .RequireAuthorization();
    }
}
