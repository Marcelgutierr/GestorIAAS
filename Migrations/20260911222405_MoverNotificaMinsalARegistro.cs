using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorIAAS.Migrations
{
    /// <inheritdoc />
    public partial class MoverNotificaMinsalARegistro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotificaMinsal",
                table: "RegistrosIAAS",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificaMinsal",
                table: "RegistrosIAAS");
        }
    }
}
