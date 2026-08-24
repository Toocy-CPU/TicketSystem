using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Database.Migrations
{
    /// <inheritdoc />
    public partial class Version7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AbsenderAnzeigen",
                table: "Mails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmpfangerAnzeigen",
                table: "Mails",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbsenderAnzeigen",
                table: "Mails");

            migrationBuilder.DropColumn(
                name: "EmpfangerAnzeigen",
                table: "Mails");
        }
    }
}
