using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorIAAS.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDotOriginal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DotOriginal",
                table: "RegistrosIAAS",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DotOriginal",
                table: "RegistrosIAAS");
        }
    }
}
