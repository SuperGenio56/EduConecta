using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EduConecta.Data.Migrations
{
    /// <inheritdoc />
    public partial class OfertaAcademica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CodigoCurso = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    NombreCurso = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Creditos = table.Column<int>(type: "int", nullable: false),
                    Modalidad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CupoMaximo = table.Column<int>(type: "int", nullable: false),
                    CostoMatricula = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                    table.CheckConstraint("CK_Cursos_CupoMaximo", "[CupoMaximo] BETWEEN 1 AND 500");
                });

            migrationBuilder.CreateTable(
                name: "Docentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCompleto = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Especialidad = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docentes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CursosPrerrequisitos",
                columns: table => new
                {
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    PrerrequisitoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CursosPrerrequisitos", x => new { x.CursoId, x.PrerrequisitoId });
                    table.CheckConstraint("CK_CursosPrerrequisitos_NoEsPrerrequisitoDeSiMismo", "[CursoId] <> [PrerrequisitoId]");
                    table.ForeignKey(
                        name: "FK_CursosPrerrequisitos_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CursosPrerrequisitos_Cursos_PrerrequisitoId",
                        column: x => x.PrerrequisitoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Modulos",
                columns: table => new
                {
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    NumeroModulo = table.Column<int>(type: "int", nullable: false),
                    NombreModulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    HorasEstimadas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modulos", x => new { x.CursoId, x.NumeroModulo });
                    table.ForeignKey(
                        name: "FK_Modulos_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CursosDocentes",
                columns: table => new
                {
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    DocenteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CursosDocentes", x => new { x.CursoId, x.DocenteId });
                    table.ForeignKey(
                        name: "FK_CursosDocentes_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CursosDocentes_Docentes_DocenteId",
                        column: x => x.DocenteId,
                        principalTable: "Docentes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cursos",
                columns: new[] { "Id", "CodigoCurso", "CostoMatricula", "Creditos", "CupoMaximo", "Modalidad", "NombreCurso" },
                values: new object[,]
                {
                    { 1, "WEB-301", 750m, 4, 30, "Presencial", "Programacion Web III" },
                    { 2, "WEB-201", 700m, 4, 35, "Presencial", "Programacion Web II" },
                    { 3, "BD-301", 800m, 4, 25, "Hibrida", "Base de Datos III" },
                    { 4, "RED-101", 600m, 3, 40, "Virtual", "Fundamentos de Redes" }
                });

            migrationBuilder.InsertData(
                table: "Docentes",
                columns: new[] { "Id", "Correo", "Especialidad", "NombreCompleto" },
                values: new object[,]
                {
                    { 1, "aacebo@educonecta.edu.bo", "Desarrollo web", "Agustin Acebo Pedraza" },
                    { 2, "lmendoza@educonecta.edu.bo", "Bases de datos", "Lucia Mendoza Rojas" },
                    { 3, "iquispe@educonecta.edu.bo", "Redes", "Ivan Quispe Flores" }
                });

            migrationBuilder.InsertData(
                table: "CursosDocentes",
                columns: new[] { "CursoId", "DocenteId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 2 },
                    { 4, 3 }
                });

            migrationBuilder.InsertData(
                table: "CursosPrerrequisitos",
                columns: new[] { "CursoId", "PrerrequisitoId" },
                values: new object[] { 1, 2 });

            migrationBuilder.InsertData(
                table: "Modulos",
                columns: new[] { "CursoId", "NumeroModulo", "Contenido", "HorasEstimadas", "NombreModulo" },
                values: new object[,]
                {
                    { 1, 1, "Patron MVC, rutas, controladores y vistas", 20, "Fundamentos de MVC" },
                    { 1, 2, "DbContext, migraciones y Fluent API", 24, "Entity Framework Core" },
                    { 3, 1, "Normalizacion y restricciones", 18, "Modelo relacional" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_CodigoCurso",
                table: "Cursos",
                column: "CodigoCurso",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CursosDocentes_DocenteId",
                table: "CursosDocentes",
                column: "DocenteId");

            migrationBuilder.CreateIndex(
                name: "IX_CursosPrerrequisitos_PrerrequisitoId",
                table: "CursosPrerrequisitos",
                column: "PrerrequisitoId");

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_Correo",
                table: "Docentes",
                column: "Correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CursosDocentes");

            migrationBuilder.DropTable(
                name: "CursosPrerrequisitos");

            migrationBuilder.DropTable(
                name: "Modulos");

            migrationBuilder.DropTable(
                name: "Docentes");

            migrationBuilder.DropTable(
                name: "Cursos");
        }
    }
}
