namespace INuStore.Domain.Exceptions;

/// <summary>Contains role-specific errors handled by the API exception middleware.</summary>
public static class RoleException
{
    /// <summary>Indicates that the requested role does not exist.</summary>
    public sealed class RoleNotFoundException(int roleId)
        : NotFoundException($"The role with the id {roleId} was not found.");

    /// <summary>Indicates that a role name is already in use.</summary>
    public sealed class DuplicateRoleNameException : BadRequestException
    {
        /// <summary>Creates an error for a duplicate role name detected by the service.</summary>
        public DuplicateRoleNameException() : base("Role name already exists.")
        {
        }

        /// <summary>Creates an error for a duplicate role name rejected by persistence.</summary>
        /// <param name="innerException">The original persistence failure retained for diagnostics.</param>
        public DuplicateRoleNameException(Exception innerException)
            : base("Role name already exists.", innerException)
        {
        }
    }

    /// <summary>Prevents deleting a role with existing user assignments.</summary>
    public sealed class RoleAssignedException : BadRequestException
    {
        /// <summary>Creates an error for an assigned role detected by the service.</summary>
        public RoleAssignedException() : base("Cannot delete a role that is assigned to users.")
        {
        }

        /// <summary>Creates an error for an assigned role whose deletion was rejected by persistence.</summary>
        /// <param name="innerException">The original persistence failure retained for diagnostics.</param>
        public RoleAssignedException(Exception innerException)
            : base("Cannot delete a role that is assigned to users.", innerException)
        {
        }
    }

    /// <summary>Protects names reserved for built-in roles.</summary>
    public sealed class BuiltInRoleException()
        : BadRequestException("Built-in role names are reserved and cannot be renamed or deleted.");
}
