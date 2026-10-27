using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduConecta.Data.Migrations
{
    /// <inheritdoc />
    public partial class Matriculas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matriculas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodoAcademico = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FechaMatricula = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "GETDATE()"),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NotaFinal = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    EstudianteId = table.Column<int>(type: "int", nullable: false),
                    CursoId = table.Column<int>(type: "int", nullable: false),
                    BecaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matriculas", x => x.Id);
                    table.CheckConstraint("CK_Matriculas_NotaFinal", "[NotaFinal] IS NULL OR ([NotaFinal] >= 0 AND [NotaFinal] <= 100)");
                    table.ForeignKey(
                        name: "FK_Matriculas_Becas_BecaId",
                        column: x => x.BecaId,
                        principalTable: "Becas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Matriculas_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matriculas_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_BecaId",
                table: "Matriculas",
                column: "BecaId");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_CursoId",
                table: "Matriculas",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_EstudianteId_CursoId_PeriodoAcademico",
                table: "Matriculas",
                columns: new[] { "EstudianteId", "CursoId", "PeriodoAcademico" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_PeriodoAcademico",
                table: "Matriculas",
                column: "PeriodoAcademico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matriculas");
        }
    }
}
