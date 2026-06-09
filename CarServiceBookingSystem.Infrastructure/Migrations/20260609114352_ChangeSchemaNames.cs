using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarServiceBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSchemaNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.EnsureSchema(
                name: "booking");

            migrationBuilder.EnsureSchema(
                name: "service");

            migrationBuilder.EnsureSchema(
                name: "finance");

            migrationBuilder.EnsureSchema(
                name: "provider");

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
                name: "TechnicianWorkingHours",
                newName: "TechnicianWorkingHours",
                newSchema: "provider")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TechnicianWorkingHoursHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "TechnicianUnavailableDates",
                newName: "TechnicianUnavailableDates",
                newSchema: "provider")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TechnicianUnavailableDatesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "TechnicianServices",
                newName: "TechnicianServices",
                newSchema: "provider")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TechnicianServicesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "Technicians",
                newName: "Technicians",
                newSchema: "provider")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TechniciansHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "StripeWebhookEvents",
                newName: "StripeWebhookEvents",
                newSchema: "finance")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "StripeWebhookEventsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "Services",
                newName: "Services",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServicesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "ServicePriceRules",
                newName: "ServicePriceRules",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServicePriceRulesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "ServiceBranches",
                newName: "ServiceBranches",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBranchesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "ServiceAreaRules",
                newName: "ServiceAreaRules",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceAreaRulesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

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
                name: "PromoCodes",
                newName: "PromoCodes",
                newSchema: "finance")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "PromoCodesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "PromoCodeRedemptions",
                newName: "PromoCodeRedemptions",
                newSchema: "finance")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "PromoCodeRedemptionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payments",
                newSchema: "finance")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "PaymentsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

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
                name: "CarYears",
                newName: "CarYears",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CarYearsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "CarTrims",
                newName: "CarTrims",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CarTrimsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "Cars",
                newName: "Cars",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CarsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "CarModels",
                newName: "CarModels",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CarModelsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "CarBrands",
                newName: "CarBrands",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CarBrandsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "CancellationPolicyRules",
                newName: "CancellationPolicyRules",
                newSchema: "dbo")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CancellationPolicyRulesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "BranchWorkingHours",
                newName: "BranchWorkingHours",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BranchWorkingHoursHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "BranchServices",
                newName: "BranchServices",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BranchServicesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "BranchClosures",
                newName: "BranchClosures",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BranchClosuresHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "BranchCapacityRules",
                newName: "BranchCapacityRules",
                newSchema: "service")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BranchCapacityRulesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "Bookings",
                newName: "Bookings",
                newSchema: "booking")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BookingsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "BookingReviews",
                newName: "BookingReviews",
                newSchema: "booking")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BookingReviewsHistory")
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "TechnicianWorkingHours",
                schema: "provider",
                newName: "TechnicianWorkingHours")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TechnicianWorkingHoursHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "TechnicianUnavailableDates",
                schema: "provider",
                newName: "TechnicianUnavailableDates")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TechnicianUnavailableDatesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "TechnicianServices",
                schema: "provider",
                newName: "TechnicianServices")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TechnicianServicesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "Technicians",
                schema: "provider",
                newName: "Technicians")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "TechniciansHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "StripeWebhookEvents",
                schema: "finance",
                newName: "StripeWebhookEvents")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "StripeWebhookEventsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "Services",
                schema: "service",
                newName: "Services")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServicesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "ServicePriceRules",
                schema: "service",
                newName: "ServicePriceRules")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServicePriceRulesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "ServiceBranches",
                schema: "service",
                newName: "ServiceBranches")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceBranchesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "ServiceAreaRules",
                schema: "service",
                newName: "ServiceAreaRules")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "ServiceAreaRulesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

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
                name: "PromoCodes",
                schema: "finance",
                newName: "PromoCodes")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "PromoCodesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "PromoCodeRedemptions",
                schema: "finance",
                newName: "PromoCodeRedemptions")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "PromoCodeRedemptionsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "Payments",
                schema: "finance",
                newName: "Payments")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "PaymentsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

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
                name: "CarYears",
                schema: "service",
                newName: "CarYears")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CarYearsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "CarTrims",
                schema: "service",
                newName: "CarTrims")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CarTrimsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "Cars",
                schema: "service",
                newName: "Cars")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CarsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "CarModels",
                schema: "service",
                newName: "CarModels")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CarModelsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "CarBrands",
                schema: "service",
                newName: "CarBrands")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CarBrandsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "CancellationPolicyRules",
                schema: "dbo",
                newName: "CancellationPolicyRules")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "CancellationPolicyRulesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "BranchWorkingHours",
                schema: "service",
                newName: "BranchWorkingHours")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BranchWorkingHoursHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "BranchServices",
                schema: "service",
                newName: "BranchServices")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BranchServicesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "BranchClosures",
                schema: "service",
                newName: "BranchClosures")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BranchClosuresHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "BranchCapacityRules",
                schema: "service",
                newName: "BranchCapacityRules")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BranchCapacityRulesHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "Bookings",
                schema: "booking",
                newName: "Bookings")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BookingsHistory")
                .Annotation("SqlServer:TemporalHistoryTableSchema", "auditing")
                .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
                .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart");

            migrationBuilder.RenameTable(
                name: "BookingReviews",
                schema: "booking",
                newName: "BookingReviews")
                .Annotation("SqlServer:IsTemporal", true)
                .Annotation("SqlServer:TemporalHistoryTableName", "BookingReviewsHistory")
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
    }
}
