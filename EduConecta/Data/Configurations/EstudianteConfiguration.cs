using EduConecta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduConecta.Data.Configurations;

public class EstudianteConfiguration : IEntityTypeConfiguration<Estudiante>
{
    public void Configure(EntityTypeBuilder<Estudiante> builder)
    {
        builder.ToTable("Estudiantes");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Nombres)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(e => e.ApellidoPaterno)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(e => e.ApellidoMaterno)
            .HasMaxLength(40);

        builder.Property(e => e.FechaNacimiento)
            .HasColumnType("date");

        builder.Property(e => e.DireccionPais)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(e => e.DireccionCiudad)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(e => e.DireccionCalle)
            .IsRequired()
            .HasMaxLength(120);

        builder.HasIndex(e => new { e.ApellidoPaterno, e.ApellidoMaterno, e.Nombres });

        builder.HasOne(e => e.Sede)
            .WithMany(s => s.Estudiantes)
            .HasForeignKey(e => e.SedeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
