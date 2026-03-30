namespace events.backend.Application.Abstractions.Authentication;

public interface IUserContext
{
    string? RegistrationId { get; }
    string? Role { get; }
}
