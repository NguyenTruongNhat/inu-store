using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INuStore.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddApplicationParameter : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ApplicationParameter",
            columns: table => new
            {
                Key = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false, collation: "Latin1_General_100_CI_AS"),
                Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ApplicationParameter", x => x.Key);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ApplicationParameter");
    }
}
