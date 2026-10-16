using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContinuingEducation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialContinuingEducation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "continuing_education");

            migrationBuilder.CreateTable(
                name: "EducationPrograms",
                schema: "continuing_education",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    InstructorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalHours = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsFree = table.Column<bool>(type: "bit", nullable: false),
                    MinAttendancePercentage = table.Column<int>(type: "int", nullable: false),
                    MinPassingGrade = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationPrograms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramApplications",
                schema: "continuing_education",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AttendedHours = table.Column<int>(type: "int", nullable: false),
                    FinalGrade = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    IsCertificateEarned = table.Column<bool>(type: "bit", nullable: false),
                    CertificateNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramApplications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationPrograms_Code",
                schema: "continuing_education",
                table: "EducationPrograms",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramApplications_CertificateNumber",
                schema: "continuing_education",
                table: "ProgramApplications",
                column: "CertificateNumber",
                unique: true,
                filter: "[CertificateNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramApplications_ProgramId_ApplicantUserId",
                schema: "continuing_education",
                table: "ProgramApplications",
                columns: new[] { "ProgramId", "ApplicantUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationPrograms",
                schema: "continuing_education");

            migrationBuilder.DropTable(
                name: "ProgramApplications",
                schema: "continuing_education");
        }
    }
}
