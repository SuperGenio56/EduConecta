using EduConecta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduConecta.Data.Configurations;

public class CursoConfiguration : IEntityTypeConfiguration<Curso>
{
    public void Configure(EntityTypeBuilder<Curso> builder)
    {
        builder.ToTable("Cursos", t =>
            t.HasCheckConstraint(
                "CK_Cursos_CupoMaximo",
                "[CupoMaximo] BETWEEN 1 AND 500"));

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CodigoCurso)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(c => c.NombreCurso)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Modalidad)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(c => c.CostoMatricula)
            .HasPrecision(10, 2);

        builder.HasIndex(c => c.CodigoCurso)
            .IsUnique();

        builder.HasData(
            new Curso { Id = 1, CodigoCurso = "WEB-301", NombreCurso = "Programacion Web III", Creditos = 4, Modalidad = ModalidadCurso.Presencial, CupoMaximo = 30, CostoMatricula = 750m },
            new Curso { Id = 2, CodigoCurso = "WEB-201", NombreCurso = "Programacion Web II", Creditos = 4, Modalidad = ModalidadCurso.Presencial, CupoMaximo = 35, CostoMatricula = 700m },
            new Curso { Id = 3, CodigoCurso = "BD-301", NombreCurso = "Base de Datos III", Creditos = 4, Modalidad = ModalidadCurso.Hibrida, CupoMaximo = 25, CostoMatricula = 800m },
            new Curso { Id = 4, CodigoCurso = "RED-101", NombreCurso = "Fundamentos de Redes", Creditos = 3, Modalidad = ModalidadCurso.Virtual, CupoMaximo = 40, CostoMatricula = 600m });
    }
}
