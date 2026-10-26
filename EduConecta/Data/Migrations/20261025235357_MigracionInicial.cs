using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EduConecta.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sedes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pais = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NombreCampus = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sedes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estudiantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombres = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ApellidoPaterno = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    ApellidoMaterno = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "date", nullable: false),
                    DireccionPais = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    DireccionCiudad = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DireccionCalle = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estudiantes_Sedes_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sedes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Becas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoBeca = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PorcentajeDescuento = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    EstudianteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Becas", x => x.Id);
                    table.CheckConstraint("CK_Becas_PorcentajeDescuento", "[PorcentajeDescuento] > 0 AND [PorcentajeDescuento] <= 100");
                    table.ForeignKey(
                        name: "FK_Becas_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CorreosEstudiante",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Direccion = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    EstudianteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorreosEstudiante", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorreosEstudiante_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TelefonosEstudiante",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EstudianteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelefonosEstudiante", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TelefonosEstudiante_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Sedes",
                columns: new[] { "Id", "Ciudad", "NombreCampus", "Pais" },
                values: new object[,]
                {
                    { 1, "Cochabamba", "Campus Tupuraya", "Bolivia" },
                    { 2, "La Paz", "Campus San Miguel", "Bolivia" },
                    { 3, "Santa Cruz", "Campus Equipetrol", "Bolivia" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Becas_EstudianteId_TipoBeca",
                table: "Becas",
                columns: new[] { "EstudianteId", "TipoBeca" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CorreosEstudiante_Direccion",
                table: "CorreosEstudiante",
                column: "Direccion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CorreosEstudiante_EstudianteId",
                table: "CorreosEstudiante",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_ApellidoPaterno_ApellidoMaterno_Nombres",
                table: "Estudiantes",
                columns: new[] { "ApellidoPaterno", "ApellidoMaterno", "Nombres" });

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_SedeId",
                table: "Estudiantes",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sedes_NombreCampus",
                table: "Sedes",
                column: "NombreCampus",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TelefonosEstudiante_EstudianteId_Numero",
                table: "TelefonosEstudiante",
                columns: new[] { "EstudianteId", "Numero" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Becas");

            migrationBuilder.DropTable(
                name: "CorreosEstudiante");

            migrationBuilder.DropTable(
                name: "TelefonosEstudiante");

            migrationBuilder.DropTable(
                name: "Estudiantes");

            migrationBuilder.DropTable(
                name: "Sedes");
        }
    }
}
