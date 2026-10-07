using DoingTasks.Api.Extensions;
using DoingTasks.Api.Infrastructure;
using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;
using DoingTasks.Application.Features.Authentication.Commands.LoginByGoogle;

namespace DoingTasks.Api.Endpoints.Authentication;

internal sealed class LoginByGoogle : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/auth/login-google", async (
            LoginByGoogleCommand command,
            ICommandHandler<LoginByGoogleCommand, AuthenticationResponse> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(command, ct);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Authentication)
        .AllowAnonymous();
    }
}
