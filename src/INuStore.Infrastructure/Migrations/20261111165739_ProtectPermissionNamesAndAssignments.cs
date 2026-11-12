using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INuStore.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ProtectPermissionNamesAndAssignments : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_RolePermissions_Permissions_PermissionId",
            table: "RolePermissions");

        migrationBuilder.CreateIndex(
            name: "IX_Permissions_Name",
            table: "Permissions",
            column: "Name",
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_RolePermissions_Permissions_PermissionId",
            table: "RolePermissions",
            column: "PermissionId",
            principalTable: "Permissions",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_RolePermissions_Permissions_PermissionId",
            table: "RolePermissions");

        migrationBuilder.DropIndex(
            name: "IX_Permissions_Name",
            table: "Permissions");

        migrationBuilder.AddForeignKey(
            name: "FK_RolePermissions_Permissions_PermissionId",
            table: "RolePermissions",
            column: "PermissionId",
            principalTable: "Permissions",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
