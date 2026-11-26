using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INuStore.Infrastructure.Migrations;

/// <inheritdoc />
public partial class AddFirstLoginNotifications : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedAtUtc",
            table: "Users",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "FirstLoggedInAtUtc",
            table: "Users",
            type: "datetime2",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "Notifications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                EventType = table.Column<int>(type: "int", nullable: false),
                Channel = table.Column<int>(type: "int", nullable: false),
                Recipient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                AttemptCount = table.Column<int>(type: "int", nullable: false),
                NextAttemptAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                ProviderMessageId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notifications", x => x.Id);
                table.ForeignKey(
                    name: "FK_Notifications_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Notifications_UserId",
            table: "Notifications",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Notifications");

        migrationBuilder.DropColumn(
            name: "CreatedAtUtc",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "FirstLoggedInAtUtc",
            table: "Users");
    }
}
