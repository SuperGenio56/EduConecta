using EduConecta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduConecta.Data.Configurations;

public class SedeConfiguration : IEntityTypeConfiguration<Sede>
{
    public void Configure(EntityTypeBuilder<Sede> builder)
    {
        builder.ToTable("Sedes");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Pais)
            .IsRequired()
            .HasMaxLength(40);

        builder.Property(s => s.Ciudad)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(s => s.NombreCampus)
            .IsRequired()
            .HasMaxLength(80);

        builder.HasIndex(s => s.NombreCampus)
            .IsUnique();

        builder.HasData(
            new Sede { Id = 1, Pais = "Bolivia", Ciudad = "Cochabamba", NombreCampus = "Campus Tupuraya" },
            new Sede { Id = 2, Pais = "Bolivia", Ciudad = "La Paz", NombreCampus = "Campus San Miguel" },
            new Sede { Id = 3, Pais = "Bolivia", Ciudad = "Santa Cruz", NombreCampus = "Campus Equipetrol" });
    }
}
