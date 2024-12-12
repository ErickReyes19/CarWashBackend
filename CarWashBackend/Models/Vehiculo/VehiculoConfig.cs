using CarWashBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarWashBackend.Configurations;

public class VehiculoConfig : IEntityTypeConfiguration<Vehiculo>
{
    public void Configure(EntityTypeBuilder<Vehiculo> builder)
    {
        builder.HasKey(e => e.id).HasName("PRIMARY");

        builder.HasIndex(e => e.cliente_id, "cliente_id");

        builder.HasIndex(e => e.placa, "placa").IsUnique();

        builder.Property(e => e.id).HasMaxLength(36);
        builder.Property(e => e.activo).HasDefaultValueSql("'1'");
        builder.Property(e => e.cliente_id)
            .IsRequired()
            .HasMaxLength(36);
        builder.Property(e => e.color).HasMaxLength(30);
        builder.Property(e => e.created_at)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");
        builder.Property(e => e.marca).HasMaxLength(50);
        builder.Property(e => e.modelo).HasMaxLength(50);
        builder.Property(e => e.placa)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(e => e.updated_at)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");

        builder.HasOne(d => d.cliente).WithMany(p => p.Vehiculos)
            .HasForeignKey(d => d.cliente_id)
            .HasConstraintName("Vehiculos_ibfk_1");
    }
}
