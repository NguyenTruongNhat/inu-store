namespace INuStore.UseCase.Exceptions;

public sealed class EmailProviderConfigurationException : Exception
{
    public EmailProviderConfigurationException(string message)
        : base(message)
    {
    }

    public EmailProviderConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
