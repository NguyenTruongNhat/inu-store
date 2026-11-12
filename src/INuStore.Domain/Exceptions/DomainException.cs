namespace INuStore.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string title, string message)
        : this(title, message, null)
    {
    }

    protected DomainException(string title, string message, Exception? innerException)
        : base(message, innerException) =>
        Title = title;

    public string Title { get; }
}
