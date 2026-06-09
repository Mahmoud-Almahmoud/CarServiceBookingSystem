using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarServiceBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSchemaNames1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "security");

            migrationBuilder.RenameTable(
                name: "UserTokens",
                schema: "dbo",
                newName: "UserTokens",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "dbo",
                newName: "UserRoles",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                schema: "dbo",
                newName: "UserLogins",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                schema: "dbo",
                newName: "UserClaims",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "dbo",
                newName: "User",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "TrustedDevice",
                schema: "dbo",
                newName: "TrustedDevice",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "SecurityAuditLog",
                schema: "dbo",
                newName: "SecurityAuditLog",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "dbo",
                newName: "Roles",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                schema: "dbo",
                newName: "RoleClaims",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                schema: "dbo",
                newName: "RefreshToken",
                newSchema: "security");

            migrationBuilder.RenameTable(
                name: "IdempotencyKeys",
                schema: "finance",
                newName: "IdempotencyKeys")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "IdempotencyKeysHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "ApiKeys",
                schema: "dbo",
                newName: "ApiKeys",
                newSchema: "security")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ApiKeysHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "audit")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserTokens",
                schema: "security",
                newName: "UserTokens",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "security",
                newName: "UserRoles",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "UserLogins",
                schema: "security",
                newName: "UserLogins",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "UserClaims",
                schema: "security",
                newName: "UserClaims",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "User",
                schema: "security",
                newName: "User",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "TrustedDevice",
                schema: "security",
                newName: "TrustedDevice",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "SecurityAuditLog",
                schema: "security",
                newName: "SecurityAuditLog",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "security",
                newName: "Roles",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "RoleClaims",
                schema: "security",
                newName: "RoleClaims",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "RefreshToken",
                schema: "security",
                newName: "RefreshToken",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "IdempotencyKeys",
                newName: "IdempotencyKeys",
                newSchema: "finance")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "IdempotencyKeysHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "ApiKeys",
                schema: "security",
                newName: "ApiKeys",
                newSchema: "dbo")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ApiKeysHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "audit")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
        }
    }
}
