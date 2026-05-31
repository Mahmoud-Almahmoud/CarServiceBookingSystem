using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarServiceBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingQuoteSnapshotFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerCity",
                table: "Bookings",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerCountryCode",
                table: "Bookings",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerLatitude",
                table: "Bookings",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerLongitude",
                table: "Bookings",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DistanceKm",
                table: "Bookings",
                type: "float(10)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedTravelTimeMinutes",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceAreaRuleId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ServicePrice",
                table: "Bookings",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ServicePriceRuleId",
                table: "Bookings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Bookings",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TravelFee",
                table: "Bookings",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerCity",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerCountryCode",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerLatitude",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "CustomerLongitude",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DistanceKm",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "EstimatedTravelTimeMinutes",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ServiceAreaRuleId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ServicePrice",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ServicePriceRuleId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TravelFee",
                table: "Bookings");
        }
    }
}
