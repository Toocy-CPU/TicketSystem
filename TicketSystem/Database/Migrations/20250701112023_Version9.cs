using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Database.Migrations
{
    /// <inheritdoc />
    public partial class Version9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tickets_TicketId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_TicketId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TicketId1",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "BlockerId",
                table: "BlockTickets",
                newName: "TicketId");

            migrationBuilder.RenameColumn(
                name: "BlockedId",
                table: "BlockTickets",
                newName: "BlockedTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockTickets_BlockedTicketId",
                table: "BlockTickets",
                column: "BlockedTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockTickets_TicketId",
                table: "BlockTickets",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockTickets_Tickets_BlockedTicketId",
                table: "BlockTickets",
                column: "BlockedTicketId",
                principalTable: "Tickets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlockTickets_Tickets_TicketId",
                table: "BlockTickets",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlockTickets_Tickets_BlockedTicketId",
                table: "BlockTickets");

            migrationBuilder.DropForeignKey(
                name: "FK_BlockTickets_Tickets_TicketId",
                table: "BlockTickets");

            migrationBuilder.DropIndex(
                name: "IX_BlockTickets_BlockedTicketId",
                table: "BlockTickets");

            migrationBuilder.DropIndex(
                name: "IX_BlockTickets_TicketId",
                table: "BlockTickets");

            migrationBuilder.RenameColumn(
                name: "TicketId",
                table: "BlockTickets",
                newName: "BlockerId");

            migrationBuilder.RenameColumn(
                name: "BlockedTicketId",
                table: "BlockTickets",
                newName: "BlockedId");

            migrationBuilder.AddColumn<int>(
                name: "TicketId1",
                table: "Comments",
                type: "int",
                nullable: true);

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
    }
}
