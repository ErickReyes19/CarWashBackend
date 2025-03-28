﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace CarWashBackend.Models;

public partial class CarwashContext : DbContext
{
    public CarwashContext()
    {
    }

    public CarwashContext(DbContextOptions<CarwashContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<EstadosServicio> EstadosServicios { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    public virtual DbSet<pago> pagos { get; set; }

    public virtual DbSet<registro_servicio> registro_servicios { get; set; }

    public virtual DbSet<registro_servicio_detalle> registro_servicio_detalles { get; set; }

    public virtual DbSet<registro_servicio_vehiculo> registro_servicio_vehiculos { get; set; }
    public DbSet<Cierre> Cierres { get; set; }
    public DbSet<CierreDetalle> CierreDetalles { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<registro_servicio_detalle_producto> registro_servicio_detalle_productos { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    var mysqlHost = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
    var mysqlDatabase = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "Carwash_DB";
    var mysqlUser = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
    var mysqlPassword = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "P@ssWord.123";
    
    var connectionString = $"server={mysqlHost};database={mysqlDatabase};uid={mysqlUser};pwd={mysqlPassword}";

    optionsBuilder.UseMySql(connectionString, Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"));
}



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.correo, "correo").IsUnique();

            entity.Property(e => e.id).HasMaxLength(36);
            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.correo).HasMaxLength(100);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.genero).HasColumnType("enum('Masculino','Femenino','Otro')");
            entity.Property(e => e.nombre)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.telefono).HasMaxLength(15);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasMany(d => d.vehiculos).WithMany(p => p.clientes)
                .UsingEntity<Dictionary<string, object>>(
                    "cliente_vehiculo",
                    r => r.HasOne<Vehiculo>().WithMany()
                        .HasForeignKey("vehiculo_id")
                        .HasConstraintName("fk_vehiculo"),
                    l => l.HasOne<Cliente>().WithMany()
                        .HasForeignKey("cliente_id")
                        .HasConstraintName("fk_cliente"),
                    j =>
                    {
                        j.HasKey("cliente_id", "vehiculo_id")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("cliente_vehiculo");
                        j.HasIndex(new[] { "vehiculo_id" }, "fk_vehiculo");
                        j.HasIndex(new[] { "cliente_id", "vehiculo_id" }, "unique_cliente_vehiculo").IsUnique();
                        j.IndexerProperty<string>("cliente_id").HasMaxLength(36);
                        j.IndexerProperty<string>("vehiculo_id").HasMaxLength(36);
                    });
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");
            entity.ToTable("producto");

            // Suponiendo que 'id' es string (puede ser Guid o lo que uses)
            entity.Property(e => e.id)
                  .HasMaxLength(36);

            entity.Property(e => e.nombre)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.descripcion)
                  .HasColumnType("text");

            entity.Property(e => e.precio)
                  .HasPrecision(10, 2);

            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
        });


        modelBuilder.Entity<Empleado>()
            .HasMany(e => e.registro_servicios)
            .WithMany(rs => rs.empleados)
            .UsingEntity<Dictionary<string, object>>(
                "empleado_registro_servicio",
                r => r.HasOne<registro_servicio>()
                      .WithMany()
                      .HasForeignKey("registro_servicio_id")
                      .HasConstraintName("fk_empleado_registro_servicio_registro_servicio"),
                l => l.HasOne<Empleado>()
                      .WithMany()
                      .HasForeignKey("empleado_id")
                      .HasConstraintName("fk_empleado_registro_servicio_empleado"),
                j =>
                {
                    j.HasKey("empleado_id", "registro_servicio_id")
                     .HasName("PRIMARY")
                     .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    j.ToTable("empleado_registro_servicio");
                    j.HasIndex(new[] { "registro_servicio_id" }, "idx_registro_servicio_id");
                    j.IndexerProperty<string>("empleado_id").HasMaxLength(36);
                    j.IndexerProperty<string>("registro_servicio_id").HasMaxLength(36);
                });
        

        modelBuilder.Entity<EstadosServicio>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.Property(e => e.id).HasMaxLength(36);
            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.descripcion).HasColumnType("text");
            entity.Property(e => e.nombre)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.nombre, "nombre").IsUnique();

            entity.Property(e => e.id).HasMaxLength(36);
            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.descripcion).HasColumnType("text");
            entity.Property(e => e.nombre)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("Role");

            entity.Property(e => e.id).HasMaxLength(36);
            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.descripcion).HasColumnType("text");
            entity.Property(e => e.nombre)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasMany(d => d.permisos).WithMany(p => p.rols)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermiso",
                    r => r.HasOne<Permiso>().WithMany()
                        .HasForeignKey("permiso_id")
                        .HasConstraintName("FK_RolePermisos_Permiso"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("rol_id")
                        .HasConstraintName("FK_RolePermisos_Rol"),
                    j =>
                    {
                        j.HasKey("rol_id", "permiso_id")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("RolePermisos");
                        j.HasIndex(new[] { "permiso_id" }, "FK_RolePermisos_Permiso");
                        j.IndexerProperty<string>("rol_id").HasMaxLength(36);
                        j.IndexerProperty<string>("permiso_id").HasMaxLength(36);
                    });
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.Property(e => e.id).HasMaxLength(36);
            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.descripcion).HasColumnType("text");
            entity.Property(e => e.nombre)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.precio).HasPrecision(10, 2);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.HasMany(e => e.Productos)
                .WithMany(p => p.Servicios)
                .UsingEntity<Dictionary<string, object>>(
                    "servicio_producto",  // Nombre de la tabla de unión
                    j => j
                         .HasOne<Producto>()
                         .WithMany()
                         .HasForeignKey("producto_id")
                         .HasConstraintName("fk_servicio_producto_producto")
                         .OnDelete(DeleteBehavior.Cascade),
                    j => j
                         .HasOne<Servicio>()
                         .WithMany()
                         .HasForeignKey("servicio_id")
                         .HasConstraintName("fk_servicio_producto_servicio")
                         .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("servicio_id", "producto_id");
                        j.ToTable("servicio_producto");
                    }
                );
        });

        modelBuilder.Entity<registro_servicio_detalle_producto>()
            .HasKey(rsdp => new { rsdp.RegistroServicioDetalleId, rsdp.ProductoId });

        modelBuilder.Entity<registro_servicio_detalle_producto>(entity =>
        {
            // Configurar el campo Precio con precisión 10,2
            entity.Property(rsdp => rsdp.Precio)
                .HasPrecision(10, 2)
                .IsRequired();  // Si quieres hacerlo obligatorio, si no, elimina IsRequired()

            // Configurar el campo Total con precisión 10,2
            entity.Property(rsdp => rsdp.Total)
                .HasPrecision(10, 2)
                .IsRequired();  // Si quieres hacerlo obligatorio, si no, elimina IsRequired()
        });



        modelBuilder.Entity<registro_servicio_detalle_producto>()
            .HasOne(rsdp => rsdp.RegistroServicioDetalle)
            .WithMany(rsd => rsd.RegistroServicioDetalleProductos)
            .HasForeignKey(rsdp => rsdp.RegistroServicioDetalleId);

        modelBuilder.Entity<registro_servicio_detalle_producto>()
            .HasOne(rsdp => rsdp.Producto)
            .WithMany(p => p.RegistroServicioDetalleProductos)
            .HasForeignKey(rsdp => rsdp.ProductoId);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.role_id, "FK_Usuario_Role");

            entity.HasIndex(e => e.empleado_id, "empleado_id");

            entity.HasIndex(e => e.usuario1, "usuario").IsUnique();

            entity.Property(e => e.id).HasMaxLength(36);
            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.contrasena)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.empleado_id).HasMaxLength(36);
            entity.Property(e => e.role_id).HasMaxLength(36);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.usuario1)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("usuario");

            entity.HasOne(d => d.empleado).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.empleado_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Usuario_Empleado");

            entity.HasOne(d => d.role).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.role_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Usuario_Role");
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.placa, "placa").IsUnique();

            entity.Property(e => e.id).HasMaxLength(36);
            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.color).HasMaxLength(30);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.marca).HasMaxLength(50);
            entity.Property(e => e.modelo).HasMaxLength(50);
            entity.Property(e => e.placa)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<pago>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.registro_servicio_id, "registro_servicio_id");

            entity.Property(e => e.id).HasMaxLength(50);
            entity.Property(e => e.metodo_pago)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.monto).HasPrecision(10, 2);
            entity.Property(e => e.registro_servicio_id)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.registro_servicio).WithMany(p => p.pagos)
                .HasForeignKey(d => d.registro_servicio_id)
                .HasConstraintName("pagos_ibfk_1");
        });

        modelBuilder.Entity<registro_servicio>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("registro_servicio");

            entity.HasIndex(e => e.cliente_id, "cliente_id");
            entity.HasIndex(e => e.estado_servicio_id, "fk_estado_servicio");

            entity.Property(e => e.total)
                .HasPrecision(10, 2)
                .IsRequired()
                .HasDefaultValue(0);
            entity.Property(e => e.id).HasMaxLength(50);
            entity.Property(e => e.cliente_id)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.estado_servicio_id)
                .HasMaxLength(36)
                .IsRequired(false);  
            entity.Property(e => e.fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");            
            entity.Property(e => e.descripcion)
                .HasDefaultValueSql("Descripcion")
                .HasColumnType("Text");
            entity.Property(e => e.usuario_id).HasMaxLength(50);

            
            entity.HasOne(d => d.cliente)
                .WithMany(p => p.registro_servicios)
                .HasForeignKey(d => d.cliente_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("registro_servicio_ibfk_1");

            
            entity.HasOne(d => d.estado_servicio)
                .WithMany(p => p.registro_servicios)
                .HasForeignKey(d => d.estado_servicio_id)
                .OnDelete(DeleteBehavior.SetNull) 
                .HasConstraintName("fk_estado_servicio");

            
            entity.HasOne(d => d.Cierre)
                .WithMany(c => c.RegistroServicios)
                .HasForeignKey(d => d.CierreId) 
                .OnDelete(DeleteBehavior.SetNull);
        });





        modelBuilder.Entity<registro_servicio_detalle>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("registro_servicio_detalle");

            entity.HasIndex(e => e.registro_servicio_vehiculo_id, "registro_servicio_vehiculo_id");

            entity.HasIndex(e => e.servicio_id, "servicio_id");

            entity.Property(e => e.id).HasMaxLength(50);
            entity.Property(e => e.precio).HasPrecision(10, 2);
            entity.Property(e => e.registro_servicio_vehiculo_id)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.servicio_id)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.registro_servicio_vehiculo).WithMany(p => p.registro_servicio_detalles)
                .HasForeignKey(d => d.registro_servicio_vehiculo_id)
                .HasConstraintName("registro_servicio_detalle_ibfk_1");

            entity.HasOne(d => d.servicio).WithMany(p => p.registro_servicio_detalles)
                .HasForeignKey(d => d.servicio_id)
                .HasConstraintName("registro_servicio_detalle_ibfk_2");
        });

        modelBuilder.Entity<registro_servicio_vehiculo>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("registro_servicio_vehiculo");

            entity.HasIndex(e => e.registro_servicio_id, "registro_servicio_id1");

            entity.HasIndex(e => e.vehiculo_id, "vehiculo_id");

            entity.Property(e => e.id).HasMaxLength(50);
            entity.Property(e => e.registro_servicio_id)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.vehiculo_id)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(d => d.registro_servicio).WithMany(p => p.registro_servicio_vehiculos)
                .HasForeignKey(d => d.registro_servicio_id)
                .HasConstraintName("registro_servicio_vehiculo_ibfk_1");

            entity.HasOne(d => d.vehiculo).WithMany(p => p.registro_servicio_vehiculos)
                .HasForeignKey(d => d.vehiculo_id)
                .HasConstraintName("registro_servicio_vehiculo_ibfk_2");
        });
        modelBuilder.Entity<Cierre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cierre");

            entity.Property(e => e.Id)
                  .HasMaxLength(50);

            entity.Property(e => e.Fecha)
                  .HasColumnType("datetime")
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .IsRequired();

            entity.Property(e => e.Total)
                  .HasPrecision(10, 2)
                  .IsRequired()
                  .HasDefaultValue(0);

            
            entity.HasMany(e => e.RegistroServicios)
                  .WithOne(rs => rs.Cierre)
                  .HasForeignKey(rs => rs.CierreId)
                  .OnDelete(DeleteBehavior.SetNull); 
        });


        modelBuilder.Entity<CierreDetalle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cierre_detalle");

            entity.Property(e => e.Id)
                  .HasMaxLength(50);

            entity.Property(e => e.CierreId)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.MetodoPago)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Monto)
                  .HasPrecision(10, 2)
                  .IsRequired()
                  .HasDefaultValue(0);

            
            entity.HasOne(d => d.Cierre)
                  .WithMany(p => p.CierreDetalles)
                  .HasForeignKey(d => d.CierreId)
                  .HasConstraintName("cierre_detalle_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}