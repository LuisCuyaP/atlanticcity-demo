using MediatR;
using events.backend.API.EndPoints;
using events.backend.API.Extensions;
using events.backend.Application.Accounts;

namespace events.backend.API.EndPoints.Auth;

internal sealed class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (Request request, ISender sender, CancellationToken cancellationToken) =>
        {
            var query = new LoginAccountQuery
            {
                UserName = request.UserName,
                Password = request.Password,
                Role = request.Role,
                RegistrationId = request.RegistrationId,
                UserApplication = request.UserApplication
            };

            var result = await sender.Send(query, cancellationToken);
            return result.Match(
                token => Results.Ok(new { accessToken = token, tokenType = "Bearer" }),
                ApiResults.Problem);
        })
        .WithTags(Tags.Auth)
        .AllowAnonymous();
    }

    internal sealed class Request
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Role { get; set; }
        public string? RegistrationId { get; set; }
        public string? UserApplication { get; set; }
    }
}
