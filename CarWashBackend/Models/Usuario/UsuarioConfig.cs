using CarWashBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarWashBackend.Configurations;

public class UsuarioConfig : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(e => e.id).HasName("PRIMARY");

        builder.HasIndex(e => e.correo, "correo").IsUnique();

        builder.HasIndex(e => e.empleado_id, "empleado_id");

        builder.HasIndex(e => e.usuario1, "usuario").IsUnique();

        builder.Property(e => e.id).HasMaxLength(36);
        builder.Property(e => e.contrasena)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(e => e.correo)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(e => e.created_at)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");
        builder.Property(e => e.empleado_id).HasMaxLength(36);
        builder.Property(e => e.updated_at)
            .ValueGeneratedOnAddOrUpdate()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasColumnType("datetime");
        builder.Property(e => e.usuario1)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("usuario");

        builder.HasOne(d => d.empleado).WithMany(p => p.Usuarios)
            .HasForeignKey(d => d.empleado_id)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("Usuarios_ibfk_1");

        builder.HasMany(d => d.rols).WithMany(p => p.usuarios)
            .UsingEntity<Dictionary<string, object>>(
                "UsuarioRole",
                r => r.HasOne<Role>().WithMany()
                    .HasForeignKey("rol_id")
                    .HasConstraintName("UsuarioRoles_ibfk_2"),
                l => l.HasOne<Usuario>().WithMany()
                    .HasForeignKey("usuario_id")
                    .HasConstraintName("UsuarioRoles_ibfk_1"),
                j =>
                {
                    j.HasKey("usuario_id", "rol_id")
                        .HasName("PRIMARY")
                        .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    j.ToTable("UsuarioRoles");
                    j.HasIndex(new[] { "rol_id" }, "rol_id");
                    j.IndexerProperty<string>("usuario_id").HasMaxLength(36);
                    j.IndexerProperty<string>("rol_id").HasMaxLength(36);
                });
    }
}
