using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Database.Migrations
{
    /// <inheritdoc />
    public partial class Version8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockingTickets",
                table: "Tickets");

            migrationBuilder.AddColumn<int>(
                name: "TicketId1",
                table: "Comments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BlockTickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlockerId = table.Column<int>(type: "int", nullable: false),
                    BlockedId = table.Column<int>(type: "int", nullable: false),
                    BlocketAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockTickets", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TicketId1",
                table: "Comments",
                column: "TicketId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tickets_TicketId1",
                table: "Comments",
                column: "TicketId1",
                principalTable: "Tickets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tickets_TicketId1",
                table: "Comments");

            migrationBuilder.DropTable(
                name: "BlockTickets");

            migrationBuilder.DropIndex(
                name: "IX_Comments_TicketId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TicketId1",
                table: "Comments");

            migrationBuilder.AddColumn<string>(
                name: "BlockingTickets",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
