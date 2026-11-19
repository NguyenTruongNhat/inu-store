namespace INuStore.Domain.Exceptions;

public static class UserException
{
    public sealed class DuplicateUserNameException()
        : ConflictException("Username already exists.");

    public sealed class EmailAccountLimitExceededException()
        : ConflictException("Email is already used by two accounts.");

    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(int userId)
            : base($"The user with the id {userId} was not found.") { }
    }
}
