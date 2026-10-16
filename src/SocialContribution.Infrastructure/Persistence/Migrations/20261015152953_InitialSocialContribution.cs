using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialContribution.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSocialContribution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SocialContributionProjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CoordinatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoordinatorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TargetAudience = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PartnerOrganization = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VolunteerQuota = table.Column<int>(type: "int", nullable: false),
                    ApprovedVolunteerCount = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ImpactSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    BeneficiaryCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialContributionProjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectVolunteers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentOrStaffNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AppliedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EvaluatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectVolunteers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectVolunteers_SocialContributionProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "SocialContributionProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectVolunteers_ProjectId",
                table: "ProjectVolunteers",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectVolunteers");

            migrationBuilder.DropTable(
                name: "SocialContributionProjects");
        }
    }
}
