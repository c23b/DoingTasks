using DoingTasks.Application.Abstractions.Authentication;
using DoingTasks.Application.Abstractions.Data;
using DoingTasks.Application.Abstractions.Messaging;
using DoingTasks.Application.DTOs.Authentication;
using DoingTasks.Application.Errors;
using DoingTasks.Domain.Users;
using DoingTasks.SharedKernel.Results;

namespace DoingTasks.Application.Features.Authentication.Commands.RegisterByGoogle;

public sealed class RegisterByGoogleCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IGoogleAuthProvider googleAuthProvider,
    IIdentityProvider identityProvider) : ICommandHandler<RegisterByGoogleCommand, AuthenticationResponse>
{
    public async Task<Result<AuthenticationResponse>> Handle(
        RegisterByGoogleCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Autentica no Google
        var googleResult = await googleAuthProvider.AuthenticateAsync(
            command.Code, 
            command.RedirectUri, 
            cancellationToken);

        if (googleResult.IsFailure)
            return Result.Failure<AuthenticationResponse>(googleResult.Error);

        // 2. Verifica se email já existe
        var emailExists = await userRepository.ExistsByEmailAsync(googleResult.Value.Email, cancellationToken);
        if (emailExists)
            return Result.Failure<AuthenticationResponse>(UserErrors.EmailAlreadyExists);

        // 3. Cria User do domínio
        var userResult = User.Create(
            googleResult.Value.Name,
            googleResult.Value.Email,
            command.Nickname,
            command.BirthDate);

        if (userResult.IsFailure)
            return Result.Failure<AuthenticationResponse>(userResult.Error);

        // 4. Cria ApplicationUser no Identity
        var identityResult = await identityProvider.GoogleRegisterAsync(
            googleResult.Value.GoogleId,
            googleResult.Value.Email,
            googleResult.Value.Name,
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
