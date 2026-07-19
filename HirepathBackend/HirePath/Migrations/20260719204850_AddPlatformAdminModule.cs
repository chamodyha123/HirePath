using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirePath.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformAdminModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessRegistrationNumber",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyEmail",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Industry",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepresentativeEmail",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepresentativeName",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SuspendedAt",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompanyInvitations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyInvitations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyRegistrationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Industry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessRegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepresentativeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RepresentativeEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AdminNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRegistrationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyRegistrationRequests_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyInvitations_CompanyId",
                table: "CompanyInvitations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyInvitations_Email",
                table: "CompanyInvitations",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyInvitations_TokenHash",
                table: "CompanyInvitations",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRegistrationRequests_CompanyId",
                table: "CompanyRegistrationRequests",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyInvitations");

            migrationBuilder.DropTable(
                name: "CompanyRegistrationRequests");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Companies");

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
                name: "CompanyEmail",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Industry",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Phone",
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
                name: "Status",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "SuspendedAt",
                table: "Companies");
        }
    }
}
