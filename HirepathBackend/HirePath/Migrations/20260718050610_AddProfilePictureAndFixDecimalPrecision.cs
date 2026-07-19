using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirePath.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureAndFixDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "CandidateProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "CandidateProfiles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GitHubUrl",
                table: "CandidateProfiles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "CandidateProfiles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "CandidateProfiles",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "CandidateProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredWorkMode",
                table: "CandidateProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfilePictureId",
                table: "CandidateProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CertificateUrl",
                table: "CandidateEducations",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "CandidateEducations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "CandidateEducations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationLevel",
                table: "CandidateEducations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GPA",
                table: "CandidateEducations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "CandidateEducations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Percentage",
                table: "CandidateEducations",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerifiedBy",
                table: "CandidateEducations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProfilePicture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateProfileId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilePicture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfilePicture_CandidateProfiles_CandidateProfileId",
                        column: x => x.CandidateProfileId,
                        principalTable: "CandidateProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_ProfilePictureId",
                table: "CandidateProfiles",
                column: "ProfilePictureId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilePicture_CandidateProfileId",
                table: "ProfilePicture",
                column: "CandidateProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_ProfilePicture_ProfilePictureId",
                table: "CandidateProfiles",
                column: "ProfilePictureId",
                principalTable: "ProfilePicture",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_ProfilePicture_ProfilePictureId",
                table: "CandidateProfiles");

            migrationBuilder.DropTable(
                name: "ProfilePicture");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_ProfilePictureId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "GitHubUrl",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "PreferredWorkMode",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "ProfilePictureId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "CertificateUrl",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "City",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "EducationLevel",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "GPA",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "CandidateEducations");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "CandidateEducations");
        }
    }
}
