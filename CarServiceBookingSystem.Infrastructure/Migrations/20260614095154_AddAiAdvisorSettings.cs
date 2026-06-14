using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarServiceBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiAdvisorSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AiAdvisorSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaseUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TimeoutSeconds = table.Column<int>(type: "int", nullable: false),
                    MaxSuggestions = table.Column<int>(type: "int", nullable: false),
                    BookingPathTemplate = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MaxPromptLength = table.Column<int>(type: "int", nullable: false),
                    MinimumRecommendationConfidence = table.Column<double>(type: "float", nullable: false),
                    BlockUnrelatedQuestions = table.Column<bool>(type: "bit", nullable: false),
                    EnablePromptInjectionFilter = table.Column<bool>(type: "bit", nullable: false),
                    RateLimitPerMinute = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AiAdvisorSettings", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AiAdvisorSettings",
                columns: new[] { "Id", "BaseUrl", "BlockUnrelatedQuestions", "BookingPathTemplate", "CreatedAtUtc", "EnablePromptInjectionFilter", "Enabled", "MaxPromptLength", "MaxSuggestions", "MinimumRecommendationConfidence", "Model", "Provider", "RateLimitPerMinute", "TimeoutSeconds", "UpdatedAtUtc", "UpdatedByUserId" },
                values: new object[] { 1, "http://localhost:11434", true, "/app/bookings/new?serviceId={0}", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, true, 2000, 3, 0.45000000000000001, "llama3.1:8b", "Ollama", 10, 60, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AiAdvisorSettings");
        }
    }
}
