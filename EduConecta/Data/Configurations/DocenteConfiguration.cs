using EduConecta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduConecta.Data.Configurations;

public class DocenteConfiguration : IEntityTypeConfiguration<Docente>
{
    public void Configure(EntityTypeBuilder<Docente> builder)
    {
        builder.ToTable("Docentes");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.NombreCompleto)
            .IsRequired()
            .HasMaxLength(120);

        builder.Property(d => d.Especialidad)
            .IsRequired()
            .HasMaxLength(80);

        builder.Property(d => d.Correo)
            .IsRequired()
            .HasMaxLength(254);

        builder.HasIndex(d => d.Correo)
            .IsUnique();

        builder.HasData(
            new Docente { Id = 1, NombreCompleto = "Agustin Acebo Pedraza", Especialidad = "Desarrollo web", Correo = "aacebo@educonecta.edu.bo" },
            new Docente { Id = 2, NombreCompleto = "Lucia Mendoza Rojas", Especialidad = "Bases de datos", Correo = "lmendoza@educonecta.edu.bo" },
            new Docente { Id = 3, NombreCompleto = "Ivan Quispe Flores", Especialidad = "Redes", Correo = "iquispe@educonecta.edu.bo" });
    }
}
