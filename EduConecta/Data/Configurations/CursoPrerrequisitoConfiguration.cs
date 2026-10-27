using EduConecta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduConecta.Data.Configurations;

public class CursoPrerrequisitoConfiguration : IEntityTypeConfiguration<CursoPrerrequisito>
{
    public void Configure(EntityTypeBuilder<CursoPrerrequisito> builder)
    {
        builder.ToTable("CursosPrerrequisitos", t =>
            t.HasCheckConstraint(
                "CK_CursosPrerrequisitos_NoEsPrerrequisitoDeSiMismo",
                "[CursoId] <> [PrerrequisitoId]"));

        builder.HasKey(cp => new { cp.CursoId, cp.PrerrequisitoId });

        builder.HasOne(cp => cp.Curso)
            .WithMany(c => c.Prerrequisitos)
            .HasForeignKey(cp => cp.CursoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cp => cp.Prerrequisito)
            .WithMany(c => c.EsPrerrequisitoDe)
            .HasForeignKey(cp => cp.PrerrequisitoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new CursoPrerrequisito { CursoId = 1, PrerrequisitoId = 2 });
    }
}
