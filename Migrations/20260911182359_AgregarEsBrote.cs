using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorIAAS.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEsBrote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsBrote",
                table: "RegistrosIAAS",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosIAAS_Anio_DotOriginal",
                table: "RegistrosIAAS",
                columns: new[] { "Anio", "DotOriginal" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RegistrosIAAS_Anio_DotOriginal",
                table: "RegistrosIAAS");

            migrationBuilder.DropColumn(
                name: "EsBrote",
                table: "RegistrosIAAS");
        }
    }
}
