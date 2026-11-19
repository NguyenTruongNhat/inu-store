namespace INuStore.Domain.Exceptions;

public static class RoleException
{
    public sealed class RoleNotFoundException(int roleId)
        : NotFoundException($"The role with the id {roleId} was not found.");

    public sealed class DuplicateRoleNameException()
        : BadRequestException("Role name already exists.");

    public sealed class RoleAssignedException()
        : BadRequestException("Cannot delete a role that is assigned to users.");

    public sealed class BuiltInRoleException()
        : BadRequestException("Built-in role names are reserved and cannot be renamed or deleted.");
}
