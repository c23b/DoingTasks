using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoingTasks.Application.Features.Authentication.Commands.RefreshToken;
public sealed record RefreshTokenCommand() : ICommand<AuthenticationResponse>;
