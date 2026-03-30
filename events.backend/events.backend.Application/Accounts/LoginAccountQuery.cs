using events.backend.Application.Abstractions.Messaging;

namespace events.backend.Application.Accounts;

public class LoginAccountQuery : IQuery<string>
{
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
    public string? RegistrationId { get; set; }
    public string? UserApplication { get; set; }
}
