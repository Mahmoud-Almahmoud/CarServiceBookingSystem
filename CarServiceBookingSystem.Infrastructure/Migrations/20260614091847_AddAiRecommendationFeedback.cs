using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarServiceBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiRecommendationFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeedbackComment",
                table: "AiServiceRecommendations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FeedbackCreatedAtUtc",
                table: "AiServiceRecommendations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeedbackValue",
                table: "AiServiceRecommendations",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AiServiceRecommendations_FeedbackValue",
                table: "AiServiceRecommendations",
                column: "FeedbackValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AiServiceRecommendations_FeedbackValue",
                table: "AiServiceRecommendations");

            migrationBuilder.DropColumn(
                name: "FeedbackComment",
                table: "AiServiceRecommendations");

            migrationBuilder.DropColumn(
                name: "FeedbackCreatedAtUtc",
                table: "AiServiceRecommendations");

            migrationBuilder.DropColumn(
                name: "FeedbackValue",
                table: "AiServiceRecommendations");
        }
    }
}
