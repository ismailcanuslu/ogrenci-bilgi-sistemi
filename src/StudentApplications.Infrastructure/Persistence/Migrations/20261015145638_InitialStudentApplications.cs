using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentApplications.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialStudentApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "student_applications");

            migrationBuilder.CreateTable(
                name: "StudentApplications",
                schema: "student_applications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationNumber = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    State = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    TargetDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TargetInstitution = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gpa = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentApplications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentApplications_ApplicationNumber",
                schema: "student_applications",
                table: "StudentApplications",
                column: "ApplicationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentApplications_StudentId",
                schema: "student_applications",
                table: "StudentApplications",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentApplications",
                schema: "student_applications");
        }
    }
}
