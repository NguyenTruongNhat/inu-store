namespace INuStore.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string title, string message)
        : this(title, message, null)
    {
    }

    /// <summary>Creates a domain error while preserving its underlying cause for diagnostics.</summary>
    /// <param name="title">The safe error title.</param>
    /// <param name="message">The safe error description.</param>
    /// <param name="innerException">The underlying failure, if any.</param>
    protected DomainException(string title, string message, Exception? innerException)
        : base(message, innerException) =>
        Title = title;

    public string Title { get; }
}
