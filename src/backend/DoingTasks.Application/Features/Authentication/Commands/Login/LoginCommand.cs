using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;

namespace DoingTasks.Application.Features.Authentication.Commands.Login;
public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<AuthenticationResponse>;