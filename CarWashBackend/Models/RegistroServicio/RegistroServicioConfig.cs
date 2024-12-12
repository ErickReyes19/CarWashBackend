using CarWashBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarWashBackend.Configurations;

public class RegistroServicioConfig : IEntityTypeConfiguration<RegistroServicio>
{
    public void Configure(EntityTypeBuilder<RegistroServicio> builder)
    {
        builder.HasKey(e => e.id).HasName("PRIMARY");

        builder.HasIndex(e => e.cliente_id, "cliente_id");

        builder.HasIndex(e => e.estado_id, "estado_id");

        builder.HasIndex(e => e.servicio_id, "servicio_id");

        builder.HasIndex(e => e.usuario_id, "usuario_id");

        builder.HasIndex(e => e.vehiculo_id, "vehiculo_id");

        builder.Property(e => e.id).HasMaxLength(36);
        builder.Property(e => e.cliente_id)
            .IsRequired()
            .HasMaxLength(36);
        builder.Property(e => e.created_at)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");
        builder.Property(e => e.estado_id)
            .IsRequired()
            .HasMaxLength(36);
        builder.Property(e => e.fecha)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");
        builder.Property(e => e.observaciones).HasColumnType("text");
        builder.Property(e => e.servicio_id)
            .IsRequired()
            .HasMaxLength(36);
        builder.Property(e => e.updated_at)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");
        builder.Property(e => e.usuario_id)
            .IsRequired()
            .HasMaxLength(36);
        builder.Property(e => e.vehiculo_id)
            .IsRequired()
            .HasMaxLength(36);

        builder.HasOne(d => d.cliente).WithMany(p => p.RegistroServicios)
            .HasForeignKey(d => d.cliente_id)
            .HasConstraintName("RegistroServicios_ibfk_1");

        builder.HasOne(d => d.estado).WithMany(p => p.RegistroServicios)
            .HasForeignKey(d => d.estado_id)
            .HasConstraintName("RegistroServicios_ibfk_5");

        builder.HasOne(d => d.servicio).WithMany(p => p.RegistroServicios)
            .HasForeignKey(d => d.servicio_id)
            .HasConstraintName("RegistroServicios_ibfk_3");

        builder.HasOne(d => d.usuario).WithMany(p => p.RegistroServicios)
            .HasForeignKey(d => d.usuario_id)
            .HasConstraintName("RegistroServicios_ibfk_4");

        builder.HasOne(d => d.vehiculo).WithMany(p => p.RegistroServicios)
            .HasForeignKey(d => d.vehiculo_id)
            .HasConstraintName("RegistroServicios_ibfk_2");
    }
}
