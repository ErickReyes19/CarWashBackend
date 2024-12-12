using CarWashBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarWashBackend.Configurations;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(e => e.id).HasName("PRIMARY");

        builder.HasIndex(e => e.nombre, "nombre").IsUnique();

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

        builder.HasMany(d => d.permisos).WithMany(p => p.rols)
            .UsingEntity<Dictionary<string, object>>(
                "RolPermiso",
                r => r.HasOne<Permiso>().WithMany()
                    .HasForeignKey("permiso_id")
                    .HasConstraintName("RolPermisos_ibfk_2"),
                l => l.HasOne<Role>().WithMany()
                    .HasForeignKey("rol_id")
                    .HasConstraintName("RolPermisos_ibfk_1"),
                j =>
                {
                    j.HasKey("rol_id", "permiso_id")
                        .HasName("PRIMARY")
                        .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    j.ToTable("RolPermisos");
                    j.HasIndex(new[] { "permiso_id" }, "permiso_id");
                    j.IndexerProperty<string>("rol_id").HasMaxLength(36);
                    j.IndexerProperty<string>("permiso_id").HasMaxLength(36);
                });
    }
}
