using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;

namespace DoingTasks.Application.Features.Authentication.Commands.LoginByGoogle;

public sealed record LoginByGoogleCommand(
    string Code,
    string RedirectUri) 
    : ICommand<AuthenticationResponse>;