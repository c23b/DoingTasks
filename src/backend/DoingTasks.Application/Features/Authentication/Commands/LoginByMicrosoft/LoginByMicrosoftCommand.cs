using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;

namespace DoingTasks.Application.Features.Authentication.Commands.LoginByMicrosoft;
public sealed record LoginByMicrosoftCommand(
    string Code,
    string RedirectUri,
    string CodeVerifier) : ICommand<AuthenticationResponse>;