using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academic.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEducationCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseSyllabi",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Language = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrerequisitesText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Aim = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TextbooksAndResources = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    PlannedLearningActivities = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    RecommendedOtherTopics = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    InstructorsText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LearningOutcomesJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    TheoryHours = table.Column<int>(type: "int", nullable: false),
                    PracticeHours = table.Column<int>(type: "int", nullable: false),
                    LabHours = table.Column<int>(type: "int", nullable: false),
                    LocalCredits = table.Column<int>(type: "int", nullable: false),
                    Ects = table.Column<int>(type: "int", nullable: false),
                    LastUpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSyllabi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramProfiles",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    History = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AwardedDegree = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DegreeLevelDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    AdmissionRequirements = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    RecognitionOfPriorLearning = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    QualificationRequirements = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProgramAims = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    OccupationalProfiles = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AccessToFurtherStudies = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ExaminationRegulations = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    GraduationRequirements = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ModeOfStudy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentHeadAndContact = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Facilities = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProgramEducationalObjectives = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProgramOutcomesJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramProfiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseSyllabi_CourseCode",
                schema: "academic",
                table: "CourseSyllabi",
                column: "CourseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProgramProfiles_ProgramId",
                schema: "academic",
                table: "ProgramProfiles",
                column: "ProgramId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseSyllabi",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "ProgramProfiles",
                schema: "academic");
        }
    }
}
