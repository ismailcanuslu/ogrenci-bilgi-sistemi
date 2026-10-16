using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academic.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentPortalEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcademicCalendarEvents",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AcademicYear = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsPublic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicCalendarEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseOfferingLiveSessions",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SessionType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MeetingUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AccessPassword = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StartTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTimeUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseOfferingLiveSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentAdvisorAssignments",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAdvisorAssignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentAlumniInfos",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GraduationYear = table.Column<int>(type: "int", nullable: true),
                    CurrentCompany = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CurrentPosition = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PersonalEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsVisibleToOtherAlumni = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAlumniInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentBankDetails",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    AccountHolderName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentBankDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentDisciplinaryRecords",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PenaltyType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DecisionNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DecisionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDisciplinaryRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentHistoryRecords",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EventDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentHistoryRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentMilitaryStatuses",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PostponementEndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    AsalDocumentNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentMilitaryStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentProfilePictures",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    UploadedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentProfilePictures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentStudyExtensions",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StandardStudyTerms = table.Column<int>(type: "int", nullable: false),
                    CompletedTerms = table.Column<int>(type: "int", nullable: false),
                    MaximumAllowedTerms = table.Column<int>(type: "int", nullable: false),
                    HasAdditionalExamRights = table.Column<bool>(type: "bit", nullable: false),
                    AdditionalExamRightsCount = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentStudyExtensions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentTuitions",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TermName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsExempt = table.Column<bool>(type: "bit", nullable: false),
                    DueDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTuitions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseOfferingLiveSessions_CourseOfferingId",
                schema: "academic",
                table: "CourseOfferingLiveSessions",
                column: "CourseOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAdvisorAssignments_StudentId_IsActive",
                schema: "academic",
                table: "StudentAdvisorAssignments",
                columns: new[] { "StudentId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAlumniInfos_StudentId",
                schema: "academic",
                table: "StudentAlumniInfos",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentBankDetails_StudentId",
                schema: "academic",
                table: "StudentBankDetails",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentDisciplinaryRecords_StudentId",
                schema: "academic",
                table: "StudentDisciplinaryRecords",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentHistoryRecords_StudentId",
                schema: "academic",
                table: "StudentHistoryRecords",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentMilitaryStatuses_StudentId",
                schema: "academic",
                table: "StudentMilitaryStatuses",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentProfilePictures_StudentId",
                schema: "academic",
                table: "StudentProfilePictures",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentStudyExtensions_StudentId",
                schema: "academic",
                table: "StudentStudyExtensions",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentTuitions_StudentId_TermName",
                schema: "academic",
                table: "StudentTuitions",
                columns: new[] { "StudentId", "TermName" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicCalendarEvents",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "CourseOfferingLiveSessions",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "StudentAdvisorAssignments",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "StudentAlumniInfos",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "StudentBankDetails",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "StudentDisciplinaryRecords",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "StudentHistoryRecords",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "StudentMilitaryStatuses",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "StudentProfilePictures",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "StudentStudyExtensions",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "StudentTuitions",
                schema: "academic");
        }
    }
}
