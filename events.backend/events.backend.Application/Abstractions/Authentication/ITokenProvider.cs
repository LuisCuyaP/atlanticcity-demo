using events.backend.Domain.Accounts;

namespace events.backend.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User usuario);
}
