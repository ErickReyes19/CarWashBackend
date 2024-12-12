using CarWashBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarWashBackend.Configurations;

public class PagoConfig : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.HasKey(e => e.id).HasName("PRIMARY");

        builder.HasIndex(e => e.registro_servicio_id, "registro_servicio_id");

        builder.Property(e => e.id).HasMaxLength(36);
        builder.Property(e => e.created_at)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");
        builder.Property(e => e.fecha)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");
        builder.Property(e => e.metodo_pago)
            .IsRequired()
            .HasColumnType("enum('Efectivo','Tarjeta','Transferencia')");
        builder.Property(e => e.monto).HasPrecision(10, 2);
        builder.Property(e => e.registro_servicio_id)
            .IsRequired()
            .HasMaxLength(36);
        builder.Property(e => e.updated_at)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");

        builder.HasOne(d => d.registro_servicio).WithMany(p => p.Pagos)
            .HasForeignKey(d => d.registro_servicio_id)
            .HasConstraintName("Pagos_ibfk_1");
    }
}
