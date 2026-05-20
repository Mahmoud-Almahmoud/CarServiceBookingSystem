using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarServiceBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceFingerprintHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceFingerprintHash",
                table: "TrustedDevices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceFingerprintHash",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceFingerprintHash",
                table: "TrustedDevices");

            migrationBuilder.DropColumn(
                name: "DeviceFingerprintHash",
                table: "RefreshTokens");
        }
    }
}
