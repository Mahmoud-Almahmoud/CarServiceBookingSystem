using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarServiceBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSecuritySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TrustedDevices",
                table: "TrustedDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SecurityAuditLogs",
                table: "SecurityAuditLogs");

            migrationBuilder.RenameTable(
                name: "TrustedDevices",
                newName: "TrustedDevice",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "SecurityAuditLogs",
                newName: "SecurityAuditLog",
                newSchema: "security");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrustedDevice",
                schema: "security",
                table: "TrustedDevice",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecurityAuditLog",
                schema: "security",
                table: "SecurityAuditLog",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TrustedDevice",
                schema: "security",
                table: "TrustedDevice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SecurityAuditLog",
                schema: "security",
                table: "SecurityAuditLog");

            migrationBuilder.RenameTable(
                name: "TrustedDevice",
                schema: "security",
                newName: "TrustedDevices");

            migrationBuilder.RenameTable(
                name: "SecurityAuditLog",
                schema: "security",
                newName: "SecurityAuditLogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrustedDevices",
                table: "TrustedDevices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SecurityAuditLogs",
                table: "SecurityAuditLogs",
                column: "Id");
        }
    }
}
