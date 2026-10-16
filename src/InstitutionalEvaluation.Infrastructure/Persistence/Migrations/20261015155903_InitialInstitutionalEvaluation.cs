using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutionalEvaluation.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialInstitutionalEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvaluationMetricSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MetricKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BreakdownJson = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: true),
                    SnapshotDateUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationMetricSnapshots", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationMetricSnapshots_Category_MetricKey",
                table: "EvaluationMetricSnapshots",
                columns: new[] { "Category", "MetricKey" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluationMetricSnapshots");
        }
    }
}
