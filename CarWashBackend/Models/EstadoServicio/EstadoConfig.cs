using CarWashBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarWashBackend.Configurations;

public class EstadosServicioConfiguration : IEntityTypeConfiguration<EstadosServicio>
{
    public void Configure(EntityTypeBuilder<EstadosServicio> builder)
    {
        builder.HasKey(e => e.id).HasName("PRIMARY");

        builder.Property(e => e.id).HasMaxLength(36);
        builder.Property(e => e.created_at)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");
        builder.Property(e => e.descripcion).HasColumnType("text");
        builder.Property(e => e.nombre)
            .IsRequired()
            .HasMaxLength(50);
        builder.Property(e => e.updated_at)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");
    }
}
