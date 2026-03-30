using events.backend.CrossCutting;

namespace events.backend.Domain.Accounts;

public static class UserErrors
{
    public static Error NotAuthorized => Error.Failure("Users.NotAuthorized", "Credenciales incorrectas.");
}
