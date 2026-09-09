using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestorIAAS.Migrations
{
    /// <inheritdoc />
    public partial class InicialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiciosClinicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiciosClinicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposIAAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    NotificaMinsal = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposIAAS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosIAAS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Mes = table.Column<string>(type: "TEXT", nullable: false),
                    Anio = table.Column<int>(type: "INTEGER", nullable: false),
                    Rut = table.Column<string>(type: "TEXT", nullable: false),
                    NumeroIAAS = table.Column<int>(type: "INTEGER", nullable: false),
                    Microorganismo = table.Column<string>(type: "TEXT", nullable: false),
                    Observacion = table.Column<string>(type: "TEXT", nullable: true),
                    TipoIAASId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServicioClinicoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosIAAS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosIAAS_ServiciosClinicos_ServicioClinicoId",
                        column: x => x.ServicioClinicoId,
                        principalTable: "ServiciosClinicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosIAAS_TiposIAAS_TipoIAASId",
                        column: x => x.TipoIAASId,
                        principalTable: "TiposIAAS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosIAAS_ServicioClinicoId",
                table: "RegistrosIAAS",
                column: "ServicioClinicoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosIAAS_TipoIAASId",
                table: "RegistrosIAAS",
                column: "TipoIAASId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosIAAS");

            migrationBuilder.DropTable(
                name: "ServiciosClinicos");

            migrationBuilder.DropTable(
                name: "TiposIAAS");
        }
    }
}
