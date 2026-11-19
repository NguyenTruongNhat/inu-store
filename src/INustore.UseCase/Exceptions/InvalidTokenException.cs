using INuStore.Domain.Exceptions;

namespace INuStore.UseCase.Exceptions;

public sealed class InvalidTokenException : DomainException
{
    public InvalidTokenException()
        : base("Authentication Failure", "Invalid or expired token session.")
    {
    }
}
