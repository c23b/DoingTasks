using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.Abstractions.Data;
using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;
using DoingTasks.Application.Errors;
using DoingTasks.Domain.Users;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Features.Authentication.Commands.RegisterByMicrosoft;

public sealed class RegisterByMicrosoftCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMicrosoftAuthProvider microsoftAuthProvider,
    IIdentityProvider identityProvider) : ICommandHandler<RegisterByMicrosoftCommand, AuthenticationResponse>
{
    public async Task<Result<AuthenticationResponse>> Handle(
        RegisterByMicrosoftCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Valida o token com a Microsoft
        var microsoftResult = await microsoftAuthProvider.AuthenticateAsync(
            command.Code,
            command.RedirectUri,
            command.CodeVerifier,
            cancellationToken);

        if (microsoftResult.IsFailure)
            return Result.Failure<AuthenticationResponse>(microsoftResult.Error);

        // 2. Verifica se email já existe
        var emailExists = await userRepository.ExistsByEmailAsync(microsoftResult.Value.Email, cancellationToken);
        if (emailExists)
            return Result.Failure<AuthenticationResponse>(UserErrors.EmailAlreadyExists);

        // 3. Cria User do domínio
        var userResult = User.Create(
            microsoftResult.Value.Name,
            microsoftResult.Value.Email,
            command.Nickname,
            command.BirthDate);

        if (userResult.IsFailure)
            return Result.Failure<AuthenticationResponse>(userResult.Error);

        // 4. Cria ApplicationUser no Identity
        var identityResult = await identityProvider.MicrosoftRegisterAsync(
            microsoftResult.Value.MicrosoftId,
            microsoftResult.Value.Email,
            microsoftResult.Value.Name,
            userResult.Value.Id);

        if (identityResult.IsFailure)
            return Result.Failure<AuthenticationResponse>(identityResult.Error);

        // 5. Atualiza IdentityId
        userResult.Value.SetIdentityId(identityResult.Value.IdentityId);

        try
        {
            // 6. Persiste o User do domínio
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
