namespace INuStore.Domain.Exceptions;

/// <summary>Contains permission-specific errors handled by the API exception middleware.</summary>
public static class PermissionException
{
    /// <summary>Indicates that the requested permission does not exist.</summary>
    public sealed class PermissionNotFoundException(int permissionId)
        : NotFoundException($"The permission with the id {permissionId} was not found.");

    /// <summary>Indicates that a permission name is already in use.</summary>
    public sealed class DuplicatePermissionNameException : BadRequestException
    {
        /// <summary>Creates an error for a duplicate permission name detected by the service.</summary>
        public DuplicatePermissionNameException() : base("Permission name already exists.")
        {
        }

        /// <summary>Creates an error for a duplicate permission name rejected by persistence.</summary>
        /// <param name="innerException">The original persistence failure retained for diagnostics.</param>
        public DuplicatePermissionNameException(Exception innerException)
            : base("Permission name already exists.", innerException)
        {
        }
    }

    /// <summary>Prevents deleting a permission with existing role assignments.</summary>
    public sealed class PermissionAssignedException : BadRequestException
    {
        /// <summary>Creates an error for an assigned permission detected by the service.</summary>
        public PermissionAssignedException() : base("Cannot delete a permission that is assigned to roles.")
        {
        }

        /// <summary>Creates an error for an assigned permission whose deletion was rejected by persistence.</summary>
        /// <param name="innerException">The original persistence failure retained for diagnostics.</param>
        public PermissionAssignedException(Exception innerException)
            : base("Cannot delete a permission that is assigned to roles.", innerException)
        {
        }
    }
}
