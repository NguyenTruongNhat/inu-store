namespace INuStore.Domain.Exceptions;

public abstract class ConflictException : DomainException
{
    protected ConflictException(string message, Exception? innerException = null)
        : base("Conflict", message, innerException)
    {
    }
}
