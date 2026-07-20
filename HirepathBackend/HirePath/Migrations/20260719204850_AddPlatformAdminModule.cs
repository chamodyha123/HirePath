using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirePath.Migrations
{
    public partial class AddPlatformAdminModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add only the columns currently missing from Companies.

            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "Companies",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessRegistrationNumber",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Companies",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepresentativeEmail",
                table: "Companies",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepresentativeName",
                table: "Companies",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedAt",
                table: "Companies",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "BusinessRegistrationNumber",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "RepresentativeEmail",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "RepresentativeName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SuspendedAt",
                table: "Companies");
        }
    }
}