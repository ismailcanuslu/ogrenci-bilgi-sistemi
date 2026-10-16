using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentManagement.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDocumentManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "document_management");

            migrationBuilder.CreateTable(
                name: "OfficialDocuments",
                schema: "document_management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ContentSummary = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreatorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    VerificationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VerificationHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SignedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficialDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentWorkflowSteps",
                schema: "document_management",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    StepType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AssigneeUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ActionDateUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentWorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentWorkflowSteps_OfficialDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "document_management",
                        principalTable: "OfficialDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentWorkflowSteps_DocumentId_StepOrder",
                schema: "document_management",
                table: "DocumentWorkflowSteps",
                columns: new[] { "DocumentId", "StepOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OfficialDocuments_DocumentNumber",
                schema: "document_management",
                table: "OfficialDocuments",
                column: "DocumentNumber",
                unique: true,
                filter: "[DocumentNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OfficialDocuments_VerificationCode",
                schema: "document_management",
                table: "OfficialDocuments",
                column: "VerificationCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentWorkflowSteps",
                schema: "document_management");

            migrationBuilder.DropTable(
                name: "OfficialDocuments",
                schema: "document_management");
        }
    }
}
