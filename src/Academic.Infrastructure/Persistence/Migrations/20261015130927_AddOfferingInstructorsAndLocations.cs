using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Academic.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOfferingInstructorsAndLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_course_offerings_PrimaryInstructorId",
                schema: "academic",
                table: "course_offerings");

            migrationBuilder.DropIndex(
                name: "IX_course_offering_schedules_Classroom_DayOfWeek_StartTime_EndTime",
                schema: "academic",
                table: "course_offering_schedules");

            migrationBuilder.DropColumn(
                name: "PrimaryInstructorId",
                schema: "academic",
                table: "course_offerings");

            migrationBuilder.AddColumn<string>(
                name: "BuildingOrFaculty",
                schema: "academic",
                table: "course_offering_schedules",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "InstructorId",
                schema: "academic",
                table: "course_offering_schedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SessionType",
                schema: "academic",
                table: "course_offering_schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "course_offering_instructors",
                schema: "academic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseOfferingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    RoleOrTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AssignedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_course_offering_instructors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_course_offering_instructors_course_offerings_CourseOfferingId",
                        column: x => x.CourseOfferingId,
                        principalSchema: "academic",
                        principalTable: "course_offerings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_course_offering_schedules_BuildingOrFaculty_Classroom_DayOfWeek_StartTime_EndTime",
                schema: "academic",
                table: "course_offering_schedules",
                columns: new[] { "BuildingOrFaculty", "Classroom", "DayOfWeek", "StartTime", "EndTime" });

            migrationBuilder.CreateIndex(
                name: "IX_course_offering_schedules_InstructorId",
                schema: "academic",
                table: "course_offering_schedules",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_course_offering_instructors_CourseOfferingId_InstructorId",
                schema: "academic",
                table: "course_offering_instructors",
                columns: new[] { "CourseOfferingId", "InstructorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_offering_instructors_InstructorId",
                schema: "academic",
                table: "course_offering_instructors",
                column: "InstructorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "course_offering_instructors",
                schema: "academic");

            migrationBuilder.DropIndex(
                name: "IX_course_offering_schedules_BuildingOrFaculty_Classroom_DayOfWeek_StartTime_EndTime",
                schema: "academic",
                table: "course_offering_schedules");

            migrationBuilder.DropIndex(
                name: "IX_course_offering_schedules_InstructorId",
                schema: "academic",
                table: "course_offering_schedules");

            migrationBuilder.DropColumn(
                name: "BuildingOrFaculty",
                schema: "academic",
                table: "course_offering_schedules");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                schema: "academic",
                table: "course_offering_schedules");

            migrationBuilder.DropColumn(
                name: "SessionType",
                schema: "academic",
                table: "course_offering_schedules");

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryInstructorId",
                schema: "academic",
                table: "course_offerings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_course_offerings_PrimaryInstructorId",
                schema: "academic",
                table: "course_offerings",
                column: "PrimaryInstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_course_offering_schedules_Classroom_DayOfWeek_StartTime_EndTime",
                schema: "academic",
                table: "course_offering_schedules",
                columns: new[] { "Classroom", "DayOfWeek", "StartTime", "EndTime" });
        }
    }
}
