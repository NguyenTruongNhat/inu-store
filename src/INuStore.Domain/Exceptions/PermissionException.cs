namespace INuStore.Domain.Exceptions;

public static class PermissionException
{
    public sealed class PermissionNotFoundException(int permissionId)
        : NotFoundException($"The permission with the id {permissionId} was not found.");

    public sealed class DuplicatePermissionNameException()
        : BadRequestException("Permission name already exists.");

    public sealed class PermissionAssignedException()
        : BadRequestException("Cannot delete a permission that is assigned to roles.");
}
