using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarServiceBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Services",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CarYears",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CarYears",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CarYears",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CarYears",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CarTrims",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CarTrims",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CarTrims",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CarTrims",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Cars",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CarModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CarModels",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CarModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CarModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CarBrands",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CarBrands",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "CarBrands",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CarBrands",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CarYears");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CarYears");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CarYears");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CarYears");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CarTrims");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CarTrims");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CarTrims");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CarTrims");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CarModels");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CarModels");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CarModels");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CarModels");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CarBrands");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CarBrands");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CarBrands");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CarBrands");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Bookings");
        }
    }
}
