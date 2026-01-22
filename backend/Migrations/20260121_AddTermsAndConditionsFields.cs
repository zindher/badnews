using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BadNews.Migrations
{
    /// <inheritdoc />
    public partial class AddTermsAndConditionsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TermsAcceptedAt",
                table: "Users",
                type: "datetime2",
                nullable: true,
                comment: "When user accepted the Terms and Conditions");

            migrationBuilder.AddColumn<string>(
                name: "TermsAcceptedVersion",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                defaultValue: "1.0",
                comment: "Version of T&C accepted by user");

            // Create index for terms acceptance tracking
            migrationBuilder.CreateIndex(
                name: "IX_Users_TermsAcceptedAt",
                table: "Users",
                column: "TermsAcceptedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_TermsAcceptedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TermsAcceptedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TermsAcceptedVersion",
                table: "Users");
        }
    }
}
