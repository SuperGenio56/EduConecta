using EduConecta.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduConecta.Data.Configurations;

public class BecaConfiguration : IEntityTypeConfiguration<Beca>
{
    public void Configure(EntityTypeBuilder<Beca> builder)
    {
        builder.ToTable("Becas", t =>
            t.HasCheckConstraint(
                "CK_Becas_PorcentajeDescuento",
                "[PorcentajeDescuento] > 0 AND [PorcentajeDescuento] <= 100"));

        builder.HasKey(b => b.Id);

        builder.Property(b => b.TipoBeca)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(b => b.PorcentajeDescuento)
            .HasPrecision(5, 2);

        builder.HasIndex(b => new { b.EstudianteId, b.TipoBeca })
            .IsUnique();

        builder.HasOne(b => b.Estudiante)
            .WithMany(e => e.Becas)
            .HasForeignKey(b => b.EstudianteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
