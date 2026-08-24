using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Database.Migrations
{
    /// <inheritdoc />
    public partial class Version5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TicketClosed",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProjectClosed",
                table: "Projects",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketClosed",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ProjectClosed",
                table: "Projects");
        }
    }
}
