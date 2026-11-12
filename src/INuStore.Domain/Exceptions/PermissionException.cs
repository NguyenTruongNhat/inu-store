namespace INuStore.Domain.Exceptions;

public static class PermissionException
{
    public sealed class PermissionNotFoundException(int permissionId)
        : NotFoundException($"The permission with the id {permissionId} was not found.");

    public sealed class DuplicatePermissionNameException : BadRequestException
    {
        public DuplicatePermissionNameException() : base("Permission name already exists.")
        {
        }

        public DuplicatePermissionNameException(Exception innerException)
            : base("Permission name already exists.", innerException)
        {
        }
    }

    public sealed class PermissionAssignedException : BadRequestException
    {
        public PermissionAssignedException() : base("Cannot delete a permission that is assigned to roles.")
        {
        }

        public PermissionAssignedException(Exception innerException)
            : base("Cannot delete a permission that is assigned to roles.", innerException)
        {
        }
    }
}
