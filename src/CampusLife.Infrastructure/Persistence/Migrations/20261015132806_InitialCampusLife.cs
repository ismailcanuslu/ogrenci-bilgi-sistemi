using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusLife.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCampusLife : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "campus_life");

            migrationBuilder.CreateTable(
                name: "facility_reservations",
                schema: "campus_life",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacilityType = table.Column<int>(type: "int", nullable: false),
                    FacilityName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ReservationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facility_reservations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "student_clubs",
                schema: "campus_life",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AdvisorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PresidentStudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_student_clubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "club_events",
                schema: "campus_life",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClubId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EventDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_club_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_club_events_student_clubs_ClubId",
                        column: x => x.ClubId,
                        principalSchema: "campus_life",
                        principalTable: "student_clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_club_events_ClubId",
                schema: "campus_life",
                table: "club_events",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_club_events_EventDateUtc",
                schema: "campus_life",
                table: "club_events",
                column: "EventDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_club_events_Status",
                schema: "campus_life",
                table: "club_events",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_facility_reservations_FacilityName_ReservationDate",
                schema: "campus_life",
                table: "facility_reservations",
                columns: new[] { "FacilityName", "ReservationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_facility_reservations_FacilityType_Status",
                schema: "campus_life",
                table: "facility_reservations",
                columns: new[] { "FacilityType", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_facility_reservations_UserId",
                schema: "campus_life",
                table: "facility_reservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_student_clubs_Code",
                schema: "campus_life",
                table: "student_clubs",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_clubs_IsActive",
                schema: "campus_life",
                table: "student_clubs",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "club_events",
                schema: "campus_life");

            migrationBuilder.DropTable(
                name: "facility_reservations",
                schema: "campus_life");

            migrationBuilder.DropTable(
                name: "student_clubs",
                schema: "campus_life");
        }
    }
}
