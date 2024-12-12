using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarWashBackend.Models.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.HasKey(e => e.id).HasName("PRIMARY");

            builder.HasIndex(e => e.correo, "correo").IsUnique();

            builder.Property(e => e.id).HasMaxLength(36);
            builder.Property(e => e.activo).HasDefaultValueSql("'1'");
            builder.Property(e => e.correo).HasMaxLength(100);
            builder.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            builder.Property(e => e.genero).HasColumnType("enum('Masculino','Femenino','Otro')");
            builder.Property(e => e.nombre)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(e => e.telefono).HasMaxLength(15);
            builder.Property(e => e.updated_at)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        }
    }
}
