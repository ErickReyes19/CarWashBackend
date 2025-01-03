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

    public virtual DbSet<Pago> Pagos { get; set; }

    public virtual DbSet<Permiso> Permisos { get; set; }

    public virtual DbSet<RegistroServicio> RegistroServicios { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    public virtual DbSet<__EFMigrationsHistory> __EFMigrationsHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=Carwash;uid=root;pwd=P@ssWord.123", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.40-mysql"));

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
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.correo, "correo1").IsUnique();

            entity.Property(e => e.id).HasMaxLength(36);
            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.apellido)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.correo).HasMaxLength(100);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.genero).HasColumnType("enum('Masculino','Femenino','Otro')");
            entity.Property(e => e.nombre)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
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

        modelBuilder.Entity<Pago>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.registro_servicio_id, "registro_servicio_id");

            entity.Property(e => e.id).HasMaxLength(36);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.metodo_pago)
                .IsRequired()
                .HasColumnType("enum('Efectivo','Tarjeta','Transferencia')");
            entity.Property(e => e.monto).HasPrecision(10, 2);
            entity.Property(e => e.registro_servicio_id)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");

            entity.HasOne(d => d.registro_servicio).WithMany(p => p.Pagos)
                .HasForeignKey(d => d.registro_servicio_id)
                .HasConstraintName("Pagos_ibfk_1");
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

        modelBuilder.Entity<RegistroServicio>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.HasIndex(e => e.cliente_id, "cliente_id");

            entity.HasIndex(e => e.estado_id, "estado_id");

            entity.HasIndex(e => e.servicio_id, "servicio_id");

            entity.HasIndex(e => e.usuario_id, "usuario_id");

            entity.HasIndex(e => e.vehiculo_id, "vehiculo_id");

            entity.Property(e => e.id).HasMaxLength(36);
            entity.Property(e => e.cliente_id)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.estado_id)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.fecha)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.observaciones).HasColumnType("text");
            entity.Property(e => e.servicio_id)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.usuario_id)
                .IsRequired()
                .HasMaxLength(36);
            entity.Property(e => e.vehiculo_id)
                .IsRequired()
                .HasMaxLength(36);

            entity.HasOne(d => d.cliente).WithMany(p => p.RegistroServicios)
                .HasForeignKey(d => d.cliente_id)
                .HasConstraintName("RegistroServicios_ibfk_1");

            entity.HasOne(d => d.estado).WithMany(p => p.RegistroServicios)
                .HasForeignKey(d => d.estado_id)
                .HasConstraintName("RegistroServicios_ibfk_5");

            entity.HasOne(d => d.servicio).WithMany(p => p.RegistroServicios)
                .HasForeignKey(d => d.servicio_id)
                .HasConstraintName("RegistroServicios_ibfk_3");

            entity.HasOne(d => d.usuario).WithMany(p => p.RegistroServicios)
                .HasForeignKey(d => d.usuario_id)
                .HasConstraintName("RegistroServicios_ibfk_4");

            entity.HasOne(d => d.vehiculo).WithMany(p => p.RegistroServicios)
                .HasForeignKey(d => d.vehiculo_id)
                .HasConstraintName("RegistroServicios_ibfk_2");
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
                .ValueGeneratedOnAddOrUpdate()
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
        });

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

            entity.HasIndex(e => e.cliente_id, "cliente_id1");

            entity.HasIndex(e => e.placa, "placa").IsUnique();

            entity.Property(e => e.id).HasMaxLength(36);
            entity.Property(e => e.activo).HasDefaultValueSql("'1'");
            entity.Property(e => e.cliente_id)
                .IsRequired()
                .HasMaxLength(36);
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

            entity.HasOne(d => d.cliente).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.cliente_id)
                .HasConstraintName("Vehiculos_ibfk_1");
        });

        modelBuilder.Entity<__EFMigrationsHistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__EFMigrationsHistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion)
                .IsRequired()
                .HasMaxLength(32);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}