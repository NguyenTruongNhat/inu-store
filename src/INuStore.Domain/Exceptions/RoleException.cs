namespace INuStore.Domain.Exceptions;

public static class RoleException
{
    public sealed class RoleNotFoundException(int roleId)
        : NotFoundException($"The role with the id {roleId} was not found.");

    public sealed class DuplicateRoleNameException : BadRequestException
    {
        public DuplicateRoleNameException() : base("Role name already exists.")
        {
        }

        public DuplicateRoleNameException(Exception innerException)
            : base("Role name already exists.", innerException)
        {
        }
    }

    public sealed class RoleAssignedException : BadRequestException
    {
        public RoleAssignedException() : base("Cannot delete a role that is assigned to users.")
        {
        }

        public RoleAssignedException(Exception innerException)
            : base("Cannot delete a role that is assigned to users.", innerException)
        {
        }
    }

    public sealed class BuiltInRoleException()
        : BadRequestException("Built-in role names are reserved and cannot be renamed or deleted.");
}
