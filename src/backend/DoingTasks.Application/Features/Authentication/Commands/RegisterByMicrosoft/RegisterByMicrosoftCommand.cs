using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;

namespace DoingTasks.Application.Features.Authentication.Commands.RegisterByMicrosoft;

public sealed record RegisterByMicrosoftCommand(
    string Code,
    string RedirectUri,
    string CodeVerifier,
    string Nickname,
    DateOnly BirthDate) : ICommand<AuthenticationResponse>;
