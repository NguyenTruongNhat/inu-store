using INuStore.Domain.Exceptions;

namespace INustore.UseCase.Exceptions;

public sealed class InvalidCredentialsException : DomainException
{
    public InvalidCredentialsException()
        : base("Authentication Failure", "Invalid username or password.")
    {
    }
}
