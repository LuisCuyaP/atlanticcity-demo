using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using events.backend.Application.Abstractions.Authentication;
using events.backend.Application.Abstractions.Messaging;
using events.backend.CrossCutting;
using events.backend.Domain.Accounts;

namespace events.backend.Application.Accounts;

internal sealed class LoginAccountQueryHandler(IConfiguration configuration, ITokenProvider tokenProvider, ILogger<LoginAccountQueryHandler> logger) : IQueryHandler<LoginAccountQuery, string>
{
    public Task<Result<string>> Handle(LoginAccountQuery request, CancellationToken cancellationToken)
    {
        if (!request.UserName!.Equals(configuration["Jwt:User"]) || !request.Password.Equals(configuration["Jwt:Password"]))
        {
            logger.LogWarning("Usuario no autorizado {username}.", request.UserName);
            return Task.FromResult(Result.Failure<string>(UserErrors.NotAuthorized));
        }

        User usuario = User.Create(request.UserName, request.Password, request.Role, request.RegistrationId, request.UserApplication);
        return Task.FromResult(Result.Success(tokenProvider.Create(usuario)));
    }
}
