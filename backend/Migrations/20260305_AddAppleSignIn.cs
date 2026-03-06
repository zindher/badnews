using Microsoft.EntityFrameworkCore.Migrations;

namespace BadNews.Migrations;

public partial class AddAppleSignIn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "AppleId",
            table: "Users",
            type: "nvarchar(256)",
            maxLength: 256,
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "IsAppleLinked",
            table: "Users",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateIndex(
            name: "IX_Users_AppleId",
            table: "Users",
            column: "AppleId",
            unique: true,
            filter: "[AppleId] IS NOT NULL");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Users_AppleId",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "AppleId",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "IsAppleLinked",
            table: "Users");
    }
}
