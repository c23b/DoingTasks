using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;

namespace DoingTasks.Application.Features.Authentication.Commands.RegisterByGoogle;

public sealed record RegisterByGoogleCommand(
    string Code,
    string RedirectUri,
    string Nickname,
    DateOnly BirthDate) : ICommand<AuthenticationResponse>;
