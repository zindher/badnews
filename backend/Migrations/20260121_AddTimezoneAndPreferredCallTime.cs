using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BadNews.Migrations
{
    /// <inheritdoc />
    public partial class AddTimezoneAndPreferredCallTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredCallTime",
                table: "Orders",
                type: "nvarchar(5)",
                nullable: true,
                comment: "Preferred call time in HH:MM format (Aguascalientes timezone)");

            migrationBuilder.AddColumn<string>(
                name: "RecipientTimezone",
                table: "Orders",
                type: "nvarchar(20)",
                nullable: true,
                comment: "Timezone of the recipient (CENTRO, MONTANA, PACIFICO, NOROESTE, QUINTANA_ROO)");

            migrationBuilder.AddColumn<string>(
                name: "RecipientState",
                table: "Orders",
                type: "nvarchar(100)",
                nullable: true,
                comment: "State/region of the recipient for reference");

            // Add indexes for timezone-based queries
            migrationBuilder.CreateIndex(
                name: "IX_Orders_RecipientTimezone",
                table: "Orders",
                column: "RecipientTimezone");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PreferredCallTime",
                table: "Orders",
                column: "PreferredCallTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_RecipientTimezone",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PreferredCallTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PreferredCallTime",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RecipientTimezone",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RecipientState",
                table: "Orders");
        }
    }
}
