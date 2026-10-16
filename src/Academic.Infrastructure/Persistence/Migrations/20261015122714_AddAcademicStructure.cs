using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academic.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "academic_years",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_academic_years", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "programs",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DegreeLevel = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_programs_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "academic",
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "academic_terms",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TermType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsRegistrationOpen = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_academic_terms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_academic_terms_academic_years_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalSchema: "academic",
                        principalTable: "academic_years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "curriculums",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicYearId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_curriculums", x => x.Id);
                    table.ForeignKey(
                        name: "FK_curriculums_academic_years_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalSchema: "academic",
                        principalTable: "academic_years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_curriculums_programs_ProgramId",
                        column: x => x.ProgramId,
                        principalSchema: "academic",
                        principalTable: "programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "curriculum_versions",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurriculumId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_curriculum_versions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_curriculum_versions_curriculums_CurriculumId",
                        column: x => x.CurriculumId,
                        principalSchema: "academic",
                        principalTable: "curriculums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "curriculum_courses",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurriculumVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Ects = table.Column<int>(type: "int", nullable: false),
                    Credit = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    IsElective = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Prerequisites = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_curriculum_courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_curriculum_courses_curriculum_versions_CurriculumVersionId",
                        column: x => x.CurriculumVersionId,
                        principalSchema: "academic",
                        principalTable: "curriculum_versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_academic_terms_AcademicYearId_TermType",
                schema: "academic",
                table: "academic_terms",
                columns: new[] { "AcademicYearId", "TermType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_academic_years_Code",
                schema: "academic",
                table: "academic_years",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_curriculum_courses_CurriculumVersionId_CourseCode_Semester",
                schema: "academic",
                table: "curriculum_courses",
                columns: new[] { "CurriculumVersionId", "CourseCode", "Semester" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_curriculum_versions_CurriculumId_VersionNumber",
                schema: "academic",
                table: "curriculum_versions",
                columns: new[] { "CurriculumId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_curriculums_AcademicYearId",
                schema: "academic",
                table: "curriculums",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_curriculums_ProgramId_AcademicYearId",
                schema: "academic",
                table: "curriculums",
                columns: new[] { "ProgramId", "AcademicYearId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_programs_DepartmentId_Code",
                schema: "academic",
                table: "programs",
                columns: new[] { "DepartmentId", "Code" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "academic_terms",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "curriculum_courses",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "curriculum_versions",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "curriculums",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "academic_years",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "programs",
                schema: "academic");
        }
    }
}
