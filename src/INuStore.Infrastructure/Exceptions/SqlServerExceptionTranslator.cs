using INuStore.Domain.Entities.Identity;
using INuStore.Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace INuStore.Infrastructure.Exceptions;

internal static class SqlServerExceptionTranslator
{
    private const string UserNameIndex = "IX_Users_UserName";
    private const string RoleNameIndex = "IX_Roles_Name";
    private const string RoleAssignmentForeignKey = "FK_UserRoles_Roles_RoleId";
    private const string PermissionNameIndex = "IX_Permissions_Name";
    private const string PermissionAssignmentForeignKey = "FK_RolePermissions_Permissions_PermissionId";

    internal static DomainException? Translate(DbUpdateException exception)
    {
        if (exception.InnerException is not SqlException sqlException)
        {
            return null;
        }

        // SQL Server exposes constraint names in error messages rather than a
        // structured property. Keep that provider-specific matching here.
        foreach (SqlError error in sqlException.Errors)
        {
            if (IsDuplicateUserValue(error, exception, UserNameIndex))
            {
                return new UserException.DuplicateUserNameException(exception);
            }

            if (IsDuplicateRoleName(error, exception))
            {
                return new RoleException.DuplicateRoleNameException(exception);
            }

            if (IsAssignedRoleDeletion(error, exception))
            {
                return new RoleException.RoleAssignedException(exception);
            }

            if (IsDuplicatePermissionName(error, exception))
            {
                return new PermissionException.DuplicatePermissionNameException(exception);
            }

            if (IsAssignedPermissionDeletion(error, exception))
            {
                return new PermissionException.PermissionAssignedException(exception);
            }
        }

        return null;
    }

    private static bool IsDuplicateUserValue(SqlError error, DbUpdateException exception, string indexName)
        => error.Number is 2601 or 2627
            && error.Message.Contains($"'{indexName}'", StringComparison.Ordinal)
            && exception.Entries.Any(entry => entry.Entity is User
                && entry.State is EntityState.Added or EntityState.Modified);

    private static bool IsDuplicateRoleName(SqlError error, DbUpdateException exception)
        => error.Number is 2601 or 2627
            && error.Message.Contains($"'{RoleNameIndex}'", StringComparison.Ordinal)
            && exception.Entries.Any(entry => entry.Entity is Role
                && entry.State is EntityState.Added or EntityState.Modified);

    private static bool IsAssignedRoleDeletion(SqlError error, DbUpdateException exception)
        => error.Number == 547
            && error.Message.Contains($"\"{RoleAssignmentForeignKey}\"", StringComparison.Ordinal)
            && exception.Entries.Any(entry => entry.Entity is Role && entry.State == EntityState.Deleted);

    private static bool IsDuplicatePermissionName(SqlError error, DbUpdateException exception)
        => error.Number is 2601 or 2627
            && error.Message.Contains($"'{PermissionNameIndex}'", StringComparison.Ordinal)
            && exception.Entries.Any(entry => entry.Entity is Permission
                && entry.State is EntityState.Added or EntityState.Modified);

    private static bool IsAssignedPermissionDeletion(SqlError error, DbUpdateException exception)
        => error.Number == 547
            && error.Message.Contains($"\"{PermissionAssignmentForeignKey}\"", StringComparison.Ordinal)
            && exception.Entries.Any(entry => entry.Entity is Permission && entry.State == EntityState.Deleted);
}
