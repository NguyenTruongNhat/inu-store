using INuStore.Domain.Entities.Identity;
using INuStore.Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace INuStore.Infrastructure.Exceptions;

/// <summary>Translates known SQL Server constraint violations into safe domain errors.</summary>
internal static class SqlServerExceptionTranslator
{
    private const string RoleNameIndex = "IX_Roles_Name";
    private const string RoleAssignmentForeignKey = "FK_UserRoles_Roles_RoleId";

    /// <summary>Returns a domain error for a recognized constraint violation, otherwise null.</summary>
    /// <param name="exception">The failed EF Core save, retained as the translated error's inner exception.</param>
    /// <returns>A safe domain exception, or null when the original exception should propagate.</returns>
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
            if (IsDuplicateRoleName(error, exception))
            {
                return new RoleException.DuplicateRoleNameException(exception);
            }

            if (IsAssignedRoleDeletion(error, exception))
            {
                return new RoleException.RoleAssignedException(exception);
            }
        }

        return null;
    }

    private static bool IsDuplicateRoleName(SqlError error, DbUpdateException exception)
        => error.Number is 2601 or 2627
            && error.Message.Contains($"'{RoleNameIndex}'", StringComparison.Ordinal)
            && exception.Entries.Any(entry => entry.Entity is Role
                && entry.State is EntityState.Added or EntityState.Modified);

    private static bool IsAssignedRoleDeletion(SqlError error, DbUpdateException exception)
        => error.Number == 547
            && error.Message.Contains($"\"{RoleAssignmentForeignKey}\"", StringComparison.Ordinal)
            && exception.Entries.Any(entry => entry.Entity is Role && entry.State == EntityState.Deleted);
}
