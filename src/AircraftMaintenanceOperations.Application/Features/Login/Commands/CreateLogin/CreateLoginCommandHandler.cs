namespace AircraftMaintenanceOperations.Application.Features.Login.Commands.CreateLogin;

public class CreateLoginCommandHandler(IAuthenticationService authenticationService, IJwtTokenService jwtTokenService) : ICommandHandler<CreateLoginCommand, CreateLoginCommandResult>
{
    public async Task<CreateLoginCommandResult> Handle(CreateLoginCommand command, CancellationToken cancellationToken)
    {
        var authenticationResult = await authenticationService.AuthenticateAsync(command.UserName, command.Password);
        if (authenticationResult is null) throw new UnauthorizedAccessException("Invalid username or password");

        var accessToken = jwtTokenService.GenerateToken(
            authenticationResult.UserId,
            authenticationResult.UserName,
            authenticationResult.Roles);

        return new CreateLoginCommandResult(new LoginDto(
            accessToken.AccessToken,
            accessToken.ExpiresAt,
            authenticationResult.UserId,
            authenticationResult.UserName,
            authenticationResult.Roles));
    }
}
