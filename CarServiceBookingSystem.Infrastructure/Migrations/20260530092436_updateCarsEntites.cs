using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarServiceBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateCarsEntites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Services",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CarName",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServicePriceRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    CarBrandId = table.Column<int>(type: "int", nullable: true),
                    CarModelId = table.Column<int>(type: "int", nullable: true),
                    CarYearId = table.Column<int>(type: "int", nullable: true),
                    CarTrimId = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePriceRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicePriceRules_CarBrands_CarBrandId",
                        column: x => x.CarBrandId,
                        principalTable: "CarBrands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServicePriceRules_CarModels_CarModelId",
                        column: x => x.CarModelId,
                        principalTable: "CarModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServicePriceRules_CarTrims_CarTrimId",
                        column: x => x.CarTrimId,
                        principalTable: "CarTrims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServicePriceRules_CarYears_CarYearId",
                        column: x => x.CarYearId,
                        principalTable: "CarYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServicePriceRules_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicePriceRules_CarBrandId",
                table: "ServicePriceRules",
                column: "CarBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePriceRules_CarModelId",
                table: "ServicePriceRules",
                column: "CarModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePriceRules_CarTrimId",
                table: "ServicePriceRules",
                column: "CarTrimId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePriceRules_CarYearId",
                table: "ServicePriceRules",
                column: "CarYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePriceRules_ServiceId",
                table: "ServicePriceRules",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePriceRules_ServiceId_CarBrandId_CarModelId_CarTrimId_CarYearId_IsActive",
                table: "ServicePriceRules",
                columns: new[] { "ServiceId", "CarBrandId", "CarModelId", "CarTrimId", "CarYearId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServicePriceRules");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CarName",
                table: "Cars");
        }
    }
}
