using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirePath.Migrations
{
    /// <inheritdoc />
    public partial class FixEducationDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_ProfilePicture_ProfilePictureId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfilePicture_CandidateProfiles_CandidateProfileId",
                table: "ProfilePicture");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_ProfilePictureId",
                table: "CandidateProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfilePicture",
                table: "ProfilePicture");

            migrationBuilder.DropIndex(
                name: "IX_ProfilePicture_CandidateProfileId",
                table: "ProfilePicture");

            migrationBuilder.RenameTable(
                name: "ProfilePicture",
                newName: "ProfilePictures");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percentage",
                table: "CandidateEducations",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "GPA",
                table: "CandidateEducations",
                type: "decimal(3,2)",
                precision: 3,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfilePictures",
                table: "ProfilePictures",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_ProfilePictureId",
                table: "CandidateProfiles",
                column: "ProfilePictureId",
                unique: true,
                filter: "[ProfilePictureId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_ProfilePictures_ProfilePictureId",
                table: "CandidateProfiles",
                column: "ProfilePictureId",
                principalTable: "ProfilePictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_ProfilePictures_ProfilePictureId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_ProfilePictureId",
                table: "CandidateProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfilePictures",
                table: "ProfilePictures");

            migrationBuilder.RenameTable(
                name: "ProfilePictures",
                newName: "ProfilePicture");

            migrationBuilder.AlterColumn<decimal>(
                name: "Percentage",
                table: "CandidateEducations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldPrecision: 5,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "GPA",
                table: "CandidateEducations",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)",
                oldPrecision: 3,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfilePicture",
                table: "ProfilePicture",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProfilePicture_CandidateProfiles_CandidateProfileId",
                table: "ProfilePicture",
                column: "CandidateProfileId",
                principalTable: "CandidateProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
