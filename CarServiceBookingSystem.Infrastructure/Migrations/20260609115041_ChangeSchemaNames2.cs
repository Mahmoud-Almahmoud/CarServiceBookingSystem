using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarServiceBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSchemaNames2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CancellationPolicyRules",
                schema: "dbo",
                newName: "CancellationPolicyRules",
                newSchema: "booking")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CancellationPolicyRulesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.RenameTable(
                name: "CancellationPolicyRules",
                schema: "booking",
                newName: "CancellationPolicyRules",
                newSchema: "dbo")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CancellationPolicyRulesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");
        }
    }
}
