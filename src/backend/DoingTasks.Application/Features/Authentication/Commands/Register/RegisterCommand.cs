using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;

namespace DoingTasks.Application.Features.Authentication.Commands.Register;

public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password,
    string Nickname,
    DateOnly BirthDate) : ICommand<AuthenticationResponse>;
