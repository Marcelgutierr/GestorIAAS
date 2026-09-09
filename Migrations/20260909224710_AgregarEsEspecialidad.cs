using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorIAAS.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEsEspecialidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosIAAS_ServiciosClinicos_ServicioClinicoId",
                table: "RegistrosIAAS");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosIAAS_ServicioClinicoId",
                table: "RegistrosIAAS");

            migrationBuilder.DropColumn(
                name: "ServicioClinicoId",
                table: "RegistrosIAAS");

            migrationBuilder.AddColumn<bool>(
                name: "EsEspecialidad",
                table: "ServiciosClinicos",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RegistroIAASServicioClinico",
                columns: table => new
                {
                    RegistrosId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiciosClinicosId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroIAASServicioClinico", x => new { x.RegistrosId, x.ServiciosClinicosId });
                    table.ForeignKey(
                        name: "FK_RegistroIAASServicioClinico_RegistrosIAAS_RegistrosId",
                        column: x => x.RegistrosId,
                        principalTable: "RegistrosIAAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistroIAASServicioClinico_ServiciosClinicos_ServiciosClinicosId",
                        column: x => x.ServiciosClinicosId,
                        principalTable: "ServiciosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistroIAASServicioClinico_ServiciosClinicosId",
                table: "RegistroIAASServicioClinico",
                column: "ServiciosClinicosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistroIAASServicioClinico");

            migrationBuilder.DropColumn(
                name: "EsEspecialidad",
                table: "ServiciosClinicos");

            migrationBuilder.AddColumn<int>(
                name: "ServicioClinicoId",
                table: "RegistrosIAAS",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosIAAS_ServicioClinicoId",
                table: "RegistrosIAAS",
                column: "ServicioClinicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosIAAS_ServiciosClinicos_ServicioClinicoId",
                table: "RegistrosIAAS",
                column: "ServicioClinicoId",
                principalTable: "ServiciosClinicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
