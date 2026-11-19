using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INuStore.Infrastructure.Migrations;

public partial class ProtectUserNamesAndEmails : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Stop before altering the schema; existing accounts require an explicit data review.
        migrationBuilder.Sql("""
            IF EXISTS (SELECT 1 FROM [Users] WHERE DATALENGTH([UserName]) > 100 OR DATALENGTH([Email]) > 508)
                THROW 51000, 'Usernames or emails exceed the new column limits. Resolve existing data before retrying.', 1;

            DECLARE @Whitespace nvarchar(32) =
                NCHAR(9) + NCHAR(10) + NCHAR(11) + NCHAR(12) + NCHAR(13) + NCHAR(32) +
                NCHAR(133) + NCHAR(160) + NCHAR(5760) + NCHAR(8192) + NCHAR(8193) +
                NCHAR(8194) + NCHAR(8195) + NCHAR(8196) + NCHAR(8197) + NCHAR(8198) +
                NCHAR(8199) + NCHAR(8200) + NCHAR(8201) + NCHAR(8202) + NCHAR(8232) +
                NCHAR(8233) + NCHAR(8239) + NCHAR(8287) + NCHAR(12288);

            IF EXISTS (SELECT 1 FROM [Users]
                WHERE DATALENGTH([UserName]) <> DATALENGTH(TRIM(@Whitespace FROM [UserName]))
                   OR DATALENGTH([Email]) <> DATALENGTH(TRIM(@Whitespace FROM [Email])))
                THROW 51001, 'Usernames or emails contain leading or trailing whitespace. Resolve existing data before retrying.', 1;

            IF EXISTS (SELECT 1 FROM [Users]
                GROUP BY [UserName] COLLATE Latin1_General_100_CI_AS HAVING COUNT_BIG(*) > 1)
                THROW 51002, 'Duplicate usernames exist under the new collation. Resolve existing data before retrying.', 1;

            IF EXISTS (SELECT 1 FROM [Users]
                GROUP BY [Email] COLLATE Latin1_General_100_CI_AS HAVING COUNT_BIG(*) > 2)
                THROW 51003, 'An email is used by more than two accounts under the new collation. Resolve existing data before retrying.', 1;
            """);

        migrationBuilder.AlterColumn<string>(
            name: "UserName",
            table: "Users",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: false,
            collation: "Latin1_General_100_CI_AS",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Users",
            type: "nvarchar(254)",
            maxLength: 254,
            nullable: false,
            collation: "Latin1_General_100_CI_AS",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email");

        migrationBuilder.CreateIndex(
            name: "IX_Users_UserName",
            table: "Users",
            column: "UserName",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Users_Email",
            table: "Users");

        migrationBuilder.DropIndex(
            name: "IX_Users_UserName",
            table: "Users");

        migrationBuilder.AlterColumn<string>(
            name: "UserName",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(50)",
            oldMaxLength: 50,
            oldCollation: "Latin1_General_100_CI_AS");

        migrationBuilder.AlterColumn<string>(
            name: "Email",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(254)",
            oldMaxLength: 254,
            oldCollation: "Latin1_General_100_CI_AS");
    }
}
