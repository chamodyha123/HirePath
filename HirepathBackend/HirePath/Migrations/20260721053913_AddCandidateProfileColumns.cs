using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HirePath.Migrations
{
    /// <inheritdoc />
    public partial class AddCandidateProfileColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Interviews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PanelMembers",
                table: "Interviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Interviews",
                type: "int",
                nullable: true);


            migrationBuilder.CreateTable(
                name: "ApplicationStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationStatusHistories_AspNetUsers_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationStatusHistories_JobApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Evaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    EvaluatorId = table.Column<int>(type: "int", nullable: false),
                    ResumeScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    AIScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    InterviewScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    HiringManagerScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    OverallScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recommendations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFinalized = table.Column<bool>(type: "bit", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Evaluations_AspNetUsers_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Evaluations_JobApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InterviewId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    EvaluatorId = table.Column<int>(type: "int", nullable: false),
                    TechnicalScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    CommunicationScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    ProblemSolvingScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    CulturalFitScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    OverallScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Strengths = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weaknesses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recommendation = table.Column<int>(type: "int", nullable: false),
                    FeedbackDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSubmitted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewFeedbacks_AspNetUsers_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterviewFeedbacks_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewFeedbacks_JobApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatusHistories_ApplicationId",
                table: "ApplicationStatusHistories",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatusHistories_ChangedAt",
                table: "ApplicationStatusHistories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatusHistories_ChangedByUserId",
                table: "ApplicationStatusHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_ApplicationId",
                table: "Evaluations",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_EvaluatorId",
                table: "Evaluations",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewFeedbacks_ApplicationId",
                table: "InterviewFeedbacks",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewFeedbacks_EvaluatorId",
                table: "InterviewFeedbacks",
                column: "EvaluatorId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewFeedbacks_InterviewId",
                table: "InterviewFeedbacks",
                column: "InterviewId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ApplicationStatusHistories");

            migrationBuilder.DropTable(
                name: "Evaluations");

            migrationBuilder.DropTable(
                name: "InterviewFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "PanelMembers",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AspNetUsers");
        }
    }
}
