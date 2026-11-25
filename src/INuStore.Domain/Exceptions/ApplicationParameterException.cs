namespace INuStore.Domain.Exceptions;

public static class ApplicationParameterException
{
    public sealed class ApplicationParameterNotFoundException(string key)
        : NotFoundException($"Application parameter '{key}' was not found.");

    public sealed class DuplicateApplicationParameterKeyException(string key)
        : ConflictException($"Application parameter '{key}' already exists.");
}
