using EduConecta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduConecta.Data.Configurations;

public class CorreoEstudianteConfiguration : IEntityTypeConfiguration<CorreoEstudiante>
{
    public void Configure(EntityTypeBuilder<CorreoEstudiante> builder)
    {
        builder.ToTable("CorreosEstudiante");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Direccion)
            .IsRequired()
            .HasMaxLength(254);

        builder.HasIndex(c => c.Direccion)
            .IsUnique();

        builder.HasOne(c => c.Estudiante)
            .WithMany(e => e.Correos)
            .HasForeignKey(c => c.EstudianteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
