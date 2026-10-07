using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.Abstractions.Data;
using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;
using DoingTasks.Application.Errors;
using DoingTasks.Domain.Users;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Features.Authentication.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IIdentityProvider identityProvider) : ICommandHandler<RegisterCommand, AuthenticationResponse>
{
    public async Task<Result<AuthenticationResponse>> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Verifica se email já existe
        var emailExists = await userRepository.ExistsByEmailAsync(command.Email, cancellationToken);
        if (emailExists)
            return Result.Failure<AuthenticationResponse>(UserErrors.EmailAlreadyExists);

        // 2. Cria o User do domínio
        var userResult = User.Create(
            command.FullName,
            command.Email,
            command.Nickname,
            command.BirthDate);

        if (userResult.IsFailure)
            return Result.Failure<AuthenticationResponse>(userResult.Error);

        // 3. Cria o ApplicationUser no Identity e retorna o JWT
        var identityResult = await identityProvider.RegisterAsync(
            command.Email,
            command.Password,
            userResult.Value.Id);

        if (identityResult.IsFailure)
            return Result.Failure<AuthenticationResponse>(identityResult.Error);

        // 4. Atualiza o IdentityId no User do domínio
        userResult.Value.SetIdentityId(identityResult.Value.IdentityId);

        try
        {
            // 5. Persiste o User do domínio
            userRepository.Add(userResult.Value);
            await unitOfWork.Commit(cancellationToken);

            return Result.Success(new AuthenticationResponse(identityResult.Value.Token));
        }
        catch (Exception)
        {
            // TODO: mover para Outbox Pattern or other
            var resultDelete = await identityProvider.DeleteAsync(identityResult.Value.IdentityId);
            if (resultDelete.IsFailure)
                return Result.Failure<AuthenticationResponse>(resultDelete.Error);

            return Result.Failure<AuthenticationResponse>(AuthenticationErrors.IdentityError);
        }
    }
}
