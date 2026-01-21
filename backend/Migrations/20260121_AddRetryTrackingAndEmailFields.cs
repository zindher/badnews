using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BadNews.Migrations
{
    /// <inheritdoc />
    public partial class AddRetryTrackingAndEmailFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecipientEmail",
                table: "Orders",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Optional email for email fallback notification");

            migrationBuilder.AddColumn<int>(
                name: "RetryDay",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Current retry day (0-2) for 3-day retry period");

            migrationBuilder.AddColumn<int>(
                name: "DailyAttempts",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Number of attempts on current day (0-3)");

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstCallAttemptDate",
                table: "Orders",
                type: "datetime2",
                nullable: true,
                comment: "Date/time of first call attempt to track 3-day window");

            migrationBuilder.AddColumn<bool>(
                name: "FallbackSMSSent",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Whether SMS fallback notification was sent after 9 attempts");

            migrationBuilder.AddColumn<bool>(
                name: "FallbackEmailSent",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Whether email fallback notification was sent after 9 attempts");

            // Create index for retry queries
            migrationBuilder.CreateIndex(
                name: "IX_Orders_RetryDay_CallAttempts",
                table: "Orders",
                columns: new[] { "RetryDay", "CallAttempts" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_FirstCallAttemptDate",
                table: "Orders",
                column: "FirstCallAttemptDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_RetryDay_CallAttempts",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_FirstCallAttemptDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RecipientEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RetryDay",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "DailyAttempts",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FirstCallAttemptDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FallbackSMSSent",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "FallbackEmailSent",
                table: "Orders");
        }
    }
}
