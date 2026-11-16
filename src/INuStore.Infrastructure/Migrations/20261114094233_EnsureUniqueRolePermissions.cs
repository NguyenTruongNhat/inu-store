using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INuStore.Infrastructure.Migrations;

public partial class EnsureUniqueRolePermissions : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Keep one assignment per pair before enforcing uniqueness on existing data.
        migrationBuilder.Sql("""
            WITH DuplicateAssignments AS (
                SELECT ROW_NUMBER() OVER (PARTITION BY RoleId, PermissionId ORDER BY Id) AS RowNumber
                FROM RolePermissions
            )
            DELETE FROM DuplicateAssignments WHERE RowNumber > 1;
            """);

        migrationBuilder.DropIndex(
            name: "IX_RolePermissions_RoleId",
            table: "RolePermissions");

        migrationBuilder.CreateIndex(
            name: "IX_RolePermissions_RoleId_PermissionId",
            table: "RolePermissions",
            columns: new[] { "RoleId", "PermissionId" },
            unique: true);
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_RolePermissions_RoleId_PermissionId",
            table: "RolePermissions");

        migrationBuilder.CreateIndex(
            name: "IX_RolePermissions_RoleId",
            table: "RolePermissions",
            column: "RoleId");
    }
}
