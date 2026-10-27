using EduConecta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduConecta.Data.Configurations;

public class ModuloConfiguration : IEntityTypeConfiguration<Modulo>
{
    public void Configure(EntityTypeBuilder<Modulo> builder)
    {
        builder.ToTable("Modulos");

        builder.HasKey(m => new { m.CursoId, m.NumeroModulo });

        builder.Property(m => m.NumeroModulo)
            .ValueGeneratedNever();

        builder.Property(m => m.NombreModulo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Contenido)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(m => m.Curso)
            .WithMany(c => c.Modulos)
            .HasForeignKey(m => m.CursoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new Modulo { CursoId = 1, NumeroModulo = 1, NombreModulo = "Fundamentos de MVC", Contenido = "Patron MVC, rutas, controladores y vistas", HorasEstimadas = 20 },
            new Modulo { CursoId = 1, NumeroModulo = 2, NombreModulo = "Entity Framework Core", Contenido = "DbContext, migraciones y Fluent API", HorasEstimadas = 24 },
            new Modulo { CursoId = 3, NumeroModulo = 1, NombreModulo = "Modelo relacional", Contenido = "Normalizacion y restricciones", HorasEstimadas = 18 });
    }
}
