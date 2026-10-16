using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportDesk.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSupportDesk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "support_desk");

            migrationBuilder.CreateTable(
                name: "support_tickets",
                schema: "support_desk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    RequesterUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetUnit = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AssignedStaffUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_support_tickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ticket_messages",
                schema: "support_desk",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsFromStaff = table.Column<bool>(type: "bit", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket_messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ticket_messages_support_tickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "support_desk",
                        principalTable: "support_tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_support_tickets_RequesterUserId",
                schema: "support_desk",
                table: "support_tickets",
                column: "RequesterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_support_tickets_TargetUnit_Status",
                schema: "support_desk",
                table: "support_tickets",
                columns: new[] { "TargetUnit", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_support_tickets_TicketNumber",
                schema: "support_desk",
                table: "support_tickets",
                column: "TicketNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ticket_messages_SenderUserId",
                schema: "support_desk",
                table: "ticket_messages",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_messages_TicketId",
                schema: "support_desk",
                table: "ticket_messages",
                column: "TicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticket_messages",
                schema: "support_desk");

            migrationBuilder.DropTable(
                name: "support_tickets",
                schema: "support_desk");
        }
    }
}
