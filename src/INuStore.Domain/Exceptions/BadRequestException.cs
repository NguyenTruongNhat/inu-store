namespace INuStore.Domain.Exceptions;

public abstract class BadRequestException : DomainException
{
    protected BadRequestException(string message)
        : base("Bad Request", message)
    {
    }

    /// <summary>Creates a safe bad-request error while preserving its underlying cause.</summary>
    /// <param name="message">The description suitable for an API response.</param>
    /// <param name="innerException">The original failure retained for diagnostics.</param>
    protected BadRequestException(string message, Exception innerException)
        : base("Bad Request", message, innerException)
    {
    }
}
