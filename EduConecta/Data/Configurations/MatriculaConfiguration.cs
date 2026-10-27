using EduConecta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduConecta.Data.Configurations;

public class MatriculaConfiguration : IEntityTypeConfiguration<Matricula>
{
    public void Configure(EntityTypeBuilder<Matricula> builder)
    {
        builder.ToTable("Matriculas", t =>
            t.HasCheckConstraint(
                "CK_Matriculas_NotaFinal",
                "[NotaFinal] IS NULL OR ([NotaFinal] >= 0 AND [NotaFinal] <= 100)"));

        builder.HasKey(m => m.Id);

        builder.Property(m => m.PeriodoAcademico)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(m => m.FechaMatricula)
            .HasColumnType("date")
            .HasDefaultValueSql("GETDATE()");

        builder.Property(m => m.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.NotaFinal)
            .HasPrecision(5, 2);

        builder.HasIndex(m => new { m.EstudianteId, m.CursoId, m.PeriodoAcademico })
            .IsUnique();

        builder.HasIndex(m => m.PeriodoAcademico);

        builder.HasOne(m => m.Estudiante)
            .WithMany(e => e.Matriculas)
            .HasForeignKey(m => m.EstudianteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Curso)
            .WithMany(c => c.Matriculas)
            .HasForeignKey(m => m.CursoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Beca)
            .WithMany(b => b.Matriculas)
            .HasForeignKey(m => m.BecaId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
