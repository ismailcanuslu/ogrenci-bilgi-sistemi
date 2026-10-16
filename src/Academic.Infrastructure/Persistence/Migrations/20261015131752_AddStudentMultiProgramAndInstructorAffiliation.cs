using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academic.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentMultiProgramAndInstructorAffiliation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "instructor_department_affiliations",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AffiliationType = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    StartDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instructor_department_affiliations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_instructor_department_affiliations_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "academic",
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_instructor_department_affiliations_faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalSchema: "academic",
                        principalTable: "faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "student_program_enrollments",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurriculumVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EnrollmentYear = table.Column<int>(type: "int", nullable: false),
                    EnrollmentDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletionDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_program_enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_student_program_enrollments_curriculum_versions_CurriculumVersionId",
                        column: x => x.CurriculumVersionId,
                        principalSchema: "academic",
                        principalTable: "curriculum_versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_program_enrollments_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "academic",
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_program_enrollments_programs_ProgramId",
                        column: x => x.ProgramId,
                        principalSchema: "academic",
                        principalTable: "programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_student_program_enrollments_students_StudentId",
                        column: x => x.StudentId,
                        principalSchema: "academic",
                        principalTable: "students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_instructor_department_affiliations_DepartmentId",
                schema: "academic",
                table: "instructor_department_affiliations",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_instructor_department_affiliations_FacultyId",
                schema: "academic",
                table: "instructor_department_affiliations",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_instructor_department_affiliations_InstructorId",
                schema: "academic",
                table: "instructor_department_affiliations",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_instructor_department_affiliations_InstructorId_DepartmentId",
                schema: "academic",
                table: "instructor_department_affiliations",
                columns: new[] { "InstructorId", "DepartmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_student_program_enrollments_CurriculumVersionId",
                schema: "academic",
                table: "student_program_enrollments",
                column: "CurriculumVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_student_program_enrollments_DepartmentId",
                schema: "academic",
                table: "student_program_enrollments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_student_program_enrollments_ProgramId",
                schema: "academic",
                table: "student_program_enrollments",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_student_program_enrollments_StudentId",
                schema: "academic",
                table: "student_program_enrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_student_program_enrollments_StudentId_ProgramId",
                schema: "academic",
                table: "student_program_enrollments",
                columns: new[] { "StudentId", "ProgramId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "instructor_department_affiliations",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "student_program_enrollments",
                schema: "academic");
        }
    }
}
