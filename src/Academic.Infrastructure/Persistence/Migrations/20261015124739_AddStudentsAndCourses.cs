using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academic.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentsAndCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "courses",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DefaultEcts = table.Column<int>(type: "int", nullable: false),
                    DefaultCredit = table.Column<int>(type: "int", nullable: false),
                    Prerequisites = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_courses_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "academic",
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "students",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurriculumVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrollmentYear = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    EnrollmentDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_students_curriculum_versions_CurriculumVersionId",
                        column: x => x.CurriculumVersionId,
                        principalSchema: "academic",
                        principalTable: "curriculum_versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_students_departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalSchema: "academic",
                        principalTable: "departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_students_programs_ProgramId",
                        column: x => x.ProgramId,
                        principalSchema: "academic",
                        principalTable: "programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_offerings",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Section = table.Column<int>(type: "int", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    EnrolledCount = table.Column<int>(type: "int", nullable: false),
                    PrimaryInstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRegistrationOpen = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_offerings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_course_offerings_academic_terms_AcademicTermId",
                        column: x => x.AcademicTermId,
                        principalSchema: "academic",
                        principalTable: "academic_terms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_course_offerings_courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "academic",
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "course_offering_schedules",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Classroom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_offering_schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_course_offering_schedules_course_offerings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalSchema: "academic",
                        principalTable: "course_offerings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_offering_schedules_Classroom_DayOfWeek_StartTime_EndTime",
                schema: "academic",
                table: "course_offering_schedules",
                columns: new[] { "Classroom", "DayOfWeek", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_course_offering_schedules_CourseOfferingId",
                schema: "academic",
                table: "course_offering_schedules",
                column: "CourseOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_course_offerings_AcademicTermId",
                schema: "academic",
                table: "course_offerings",
                column: "AcademicTermId");

            migrationBuilder.CreateIndex(
                name: "IX_course_offerings_CourseId_AcademicTermId_Section",
                schema: "academic",
                table: "course_offerings",
                columns: new[] { "CourseId", "AcademicTermId", "Section" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_offerings_PrimaryInstructorId",
                schema: "academic",
                table: "course_offerings",
                column: "PrimaryInstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_courses_Code",
                schema: "academic",
                table: "courses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_courses_DepartmentId",
                schema: "academic",
                table: "courses",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_students_CurriculumVersionId",
                schema: "academic",
                table: "students",
                column: "CurriculumVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_students_DepartmentId",
                schema: "academic",
                table: "students",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_students_Email",
                schema: "academic",
                table: "students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_students_ProgramId",
                schema: "academic",
                table: "students",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_students_StudentNumber",
                schema: "academic",
                table: "students",
                column: "StudentNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_students_UserId",
                schema: "academic",
                table: "students",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_offering_schedules",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "students",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "course_offerings",
                schema: "academic");

            migrationBuilder.DropTable(
                name: "courses",
                schema: "academic");
        }
    }
}
