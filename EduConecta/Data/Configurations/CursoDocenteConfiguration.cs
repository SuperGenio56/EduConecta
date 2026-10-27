using EduConecta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduConecta.Data.Configurations;

public class CursoDocenteConfiguration : IEntityTypeConfiguration<CursoDocente>
{
    public void Configure(EntityTypeBuilder<CursoDocente> builder)
    {
        builder.ToTable("CursosDocentes");

        builder.HasKey(cd => new { cd.CursoId, cd.DocenteId });

        builder.HasOne(cd => cd.Curso)
            .WithMany(c => c.CursoDocentes)
            .HasForeignKey(cd => cd.CursoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cd => cd.Docente)
            .WithMany(d => d.CursoDocentes)
            .HasForeignKey(cd => cd.DocenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new CursoDocente { CursoId = 1, DocenteId = 1 },
            new CursoDocente { CursoId = 2, DocenteId = 1 },
            new CursoDocente { CursoId = 3, DocenteId = 2 },
            new CursoDocente { CursoId = 4, DocenteId = 3 });
    }
}
