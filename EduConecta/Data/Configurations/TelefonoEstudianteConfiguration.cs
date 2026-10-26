using EduConecta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduConecta.Data.Configurations;

public class TelefonoEstudianteConfiguration : IEntityTypeConfiguration<TelefonoEstudiante>
{
    public void Configure(EntityTypeBuilder<TelefonoEstudiante> builder)
    {
        builder.ToTable("TelefonosEstudiante");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Numero)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(t => new { t.EstudianteId, t.Numero })
            .IsUnique();

        builder.HasOne(t => t.Estudiante)
            .WithMany(e => e.Telefonos)
            .HasForeignKey(t => t.EstudianteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
