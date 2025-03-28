﻿// <auto-generated />
using System;
using CarWashBackend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CarWashBackend.Migrations
{
    [DbContext(typeof(CarwashContext))]
    [Migration("20250310203500_UpdateTableProducto")]
    partial class UpdateTableProducto
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("utf8mb4_0900_ai_ci")
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.HasCharSet(modelBuilder, "utf8mb4");
            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("CarWashBackend.Models.Cliente", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<bool?>("activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValueSql("'1'");

                    b.Property<string>("correo")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime?>("created_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("genero")
                        .HasColumnType("enum('Masculino','Femenino','Otro')");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("telefono")
                        .HasMaxLength(15)
                        .HasColumnType("varchar(15)");

                    b.Property<DateTime?>("updated_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "correo" }, "correo")
                        .IsUnique();

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("CarWashBackend.Models.Empleado", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("varchar(255)");

                    b.Property<bool?>("activo")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("apellido")
                        .HasColumnType("longtext");

                    b.Property<string>("correo")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("datetime(6)");

                    b.Property<int?>("edad")
                        .HasColumnType("int");

                    b.Property<string>("genero")
                        .HasColumnType("longtext");

                    b.Property<string>("nombre")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("updated_at")
                        .HasColumnType("datetime(6)");

                    b.HasKey("id");

                    b.ToTable("Empleados");
                });

            modelBuilder.Entity("CarWashBackend.Models.EstadosServicio", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<bool?>("activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValueSql("'1'");

                    b.Property<DateTime?>("created_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("descripcion")
                        .HasColumnType("text");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime?>("updated_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.ToTable("EstadosServicios");
                });

            modelBuilder.Entity("CarWashBackend.Models.Permiso", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<bool?>("activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValueSql("'1'");

                    b.Property<DateTime?>("created_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("descripcion")
                        .HasColumnType("text");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime?>("updated_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "nombre" }, "nombre")
                        .IsUnique();

                    b.ToTable("Permisos");
                });

            modelBuilder.Entity("CarWashBackend.Models.Role", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<bool>("activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValueSql("'1'");

                    b.Property<DateTime?>("created_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("descripcion")
                        .HasColumnType("text");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<DateTime?>("updated_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.ToTable("Role", (string)null);
                });

            modelBuilder.Entity("CarWashBackend.Models.Servicio", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<bool?>("activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValueSql("'1'");

                    b.Property<DateTime?>("created_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("descripcion")
                        .HasColumnType("text");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("precio")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTime?>("updated_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.ToTable("Servicios");
                });

            modelBuilder.Entity("CarWashBackend.Models.Usuario", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<bool?>("activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValueSql("'1'");

                    b.Property<string>("contrasena")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime?>("created_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("empleado_id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<string>("role_id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<DateTime?>("updated_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("usuario1")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("usuario");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "role_id" }, "FK_Usuario_Role");

                    b.HasIndex(new[] { "empleado_id" }, "empleado_id");

                    b.HasIndex(new[] { "usuario1" }, "usuario")
                        .IsUnique();

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("CarWashBackend.Models.Vehiculo", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<bool?>("activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValueSql("'1'");

                    b.Property<string>("color")
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.Property<DateTime?>("created_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<string>("marca")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("modelo")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("placa")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime?>("updated_at")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "placa" }, "placa")
                        .IsUnique();

                    b.ToTable("Vehiculos");
                });

            modelBuilder.Entity("CarWashBackend.Models.pago", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("metodo_pago")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<decimal>("monto")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("registro_servicio_id")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "registro_servicio_id" }, "registro_servicio_id");

                    b.ToTable("pagos");
                });

            modelBuilder.Entity("CarWashBackend.Models.registro_servicio_detalle", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<decimal>("precio")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("registro_servicio_vehiculo_id")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("servicio_id")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "registro_servicio_vehiculo_id" }, "registro_servicio_vehiculo_id");

                    b.HasIndex(new[] { "servicio_id" }, "servicio_id");

                    b.ToTable("registro_servicio_detalle", (string)null);
                });

            modelBuilder.Entity("CarWashBackend.Models.registro_servicio_vehiculo", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("registro_servicio_id")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("vehiculo_id")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.HasIndex(new[] { "registro_servicio_id" }, "registro_servicio_id1");

                    b.HasIndex(new[] { "vehiculo_id" }, "vehiculo_id");

                    b.ToTable("registro_servicio_vehiculo", (string)null);
                });

            modelBuilder.Entity("Cierre", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("Fecha")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<decimal>("Total")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)")
                        .HasDefaultValue(0m);

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.ToTable("cierre", (string)null);
                });

            modelBuilder.Entity("CierreDetalle", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("CierreId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("MetodoPago")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<decimal>("Monto")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)")
                        .HasDefaultValue(0m);

                    b.HasKey("Id")
                        .HasName("PRIMARY");

                    b.HasIndex("CierreId");

                    b.ToTable("cierre_detalle", (string)null);
                });

            modelBuilder.Entity("Producto", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<bool?>("activo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValueSql("'1'");

                    b.Property<string>("descripcion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<decimal>("precio")
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.ToTable("producto", (string)null);
                });

            modelBuilder.Entity("RolePermiso", b =>
                {
                    b.Property<string>("rol_id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<string>("permiso_id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.HasKey("rol_id", "permiso_id")
                        .HasName("PRIMARY")
                        .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                    b.HasIndex(new[] { "permiso_id" }, "FK_RolePermisos_Permiso");

                    b.ToTable("RolePermisos", (string)null);
                });

            modelBuilder.Entity("cliente_vehiculo", b =>
                {
                    b.Property<string>("cliente_id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<string>("vehiculo_id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.HasKey("cliente_id", "vehiculo_id")
                        .HasName("PRIMARY")
                        .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                    b.HasIndex(new[] { "vehiculo_id" }, "fk_vehiculo");

                    b.HasIndex(new[] { "cliente_id", "vehiculo_id" }, "unique_cliente_vehiculo")
                        .IsUnique();

                    b.ToTable("cliente_vehiculo", (string)null);
                });

            modelBuilder.Entity("empleado_registro_servicio", b =>
                {
                    b.Property<string>("empleado_id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<string>("registro_servicio_id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.HasKey("empleado_id", "registro_servicio_id")
                        .HasName("PRIMARY")
                        .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                    b.HasIndex(new[] { "registro_servicio_id" }, "idx_registro_servicio_id");

                    b.ToTable("empleado_registro_servicio", (string)null);
                });

            modelBuilder.Entity("registro_servicio", b =>
                {
                    b.Property<string>("id")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("CierreId")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("cliente_id")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("descripcion")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("Text")
                        .HasDefaultValueSql("Descripcion");

                    b.Property<string>("estado_servicio_id")
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<DateTime>("fecha")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<decimal>("total")
                        .ValueGeneratedOnAdd()
                        .HasPrecision(10, 2)
                        .HasColumnType("decimal(10,2)")
                        .HasDefaultValue(0m);

                    b.Property<string>("usuario_id")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("id")
                        .HasName("PRIMARY");

                    b.HasIndex("CierreId");

                    b.HasIndex(new[] { "cliente_id" }, "cliente_id");

                    b.HasIndex(new[] { "estado_servicio_id" }, "fk_estado_servicio");

                    b.ToTable("registro_servicio", (string)null);
                });

            modelBuilder.Entity("registro_servicio_detalle_producto", b =>
                {
                    b.Property<string>("RegistroServicioDetalleId")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("ProductoId")
                        .HasColumnType("varchar(36)");

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.HasKey("RegistroServicioDetalleId", "ProductoId");

                    b.HasIndex("ProductoId");

                    b.ToTable("registro_servicio_detalle_productos");
                });

            modelBuilder.Entity("servicio_producto", b =>
                {
                    b.Property<string>("servicio_id")
                        .HasColumnType("varchar(36)");

                    b.Property<string>("producto_id")
                        .HasColumnType("varchar(36)");

                    b.HasKey("servicio_id", "producto_id");

                    b.HasIndex("producto_id");

                    b.ToTable("servicio_producto", (string)null);
                });

            modelBuilder.Entity("CarWashBackend.Models.Usuario", b =>
                {
                    b.HasOne("CarWashBackend.Models.Empleado", "empleado")
                        .WithMany("Usuarios")
                        .HasForeignKey("empleado_id")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK_Usuario_Empleado");

                    b.HasOne("CarWashBackend.Models.Role", "role")
                        .WithMany("Usuarios")
                        .HasForeignKey("role_id")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("FK_Usuario_Role");

                    b.Navigation("empleado");

                    b.Navigation("role");
                });

            modelBuilder.Entity("CarWashBackend.Models.pago", b =>
                {
                    b.HasOne("registro_servicio", "registro_servicio")
                        .WithMany("pagos")
                        .HasForeignKey("registro_servicio_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("pagos_ibfk_1");

                    b.Navigation("registro_servicio");
                });

            modelBuilder.Entity("CarWashBackend.Models.registro_servicio_detalle", b =>
                {
                    b.HasOne("CarWashBackend.Models.registro_servicio_vehiculo", "registro_servicio_vehiculo")
                        .WithMany("registro_servicio_detalles")
                        .HasForeignKey("registro_servicio_vehiculo_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("registro_servicio_detalle_ibfk_1");

                    b.HasOne("CarWashBackend.Models.Servicio", "servicio")
                        .WithMany("registro_servicio_detalles")
                        .HasForeignKey("servicio_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("registro_servicio_detalle_ibfk_2");

                    b.Navigation("registro_servicio_vehiculo");

                    b.Navigation("servicio");
                });

            modelBuilder.Entity("CarWashBackend.Models.registro_servicio_vehiculo", b =>
                {
                    b.HasOne("registro_servicio", "registro_servicio")
                        .WithMany("registro_servicio_vehiculos")
                        .HasForeignKey("registro_servicio_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("registro_servicio_vehiculo_ibfk_1");

                    b.HasOne("CarWashBackend.Models.Vehiculo", "vehiculo")
                        .WithMany("registro_servicio_vehiculos")
                        .HasForeignKey("vehiculo_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("registro_servicio_vehiculo_ibfk_2");

                    b.Navigation("registro_servicio");

                    b.Navigation("vehiculo");
                });

            modelBuilder.Entity("CierreDetalle", b =>
                {
                    b.HasOne("Cierre", "Cierre")
                        .WithMany("CierreDetalles")
                        .HasForeignKey("CierreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("cierre_detalle_ibfk_1");

                    b.Navigation("Cierre");
                });

            modelBuilder.Entity("RolePermiso", b =>
                {
                    b.HasOne("CarWashBackend.Models.Permiso", null)
                        .WithMany()
                        .HasForeignKey("permiso_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_RolePermisos_Permiso");

                    b.HasOne("CarWashBackend.Models.Role", null)
                        .WithMany()
                        .HasForeignKey("rol_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_RolePermisos_Rol");
                });

            modelBuilder.Entity("cliente_vehiculo", b =>
                {
                    b.HasOne("CarWashBackend.Models.Cliente", null)
                        .WithMany()
                        .HasForeignKey("cliente_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_cliente");

                    b.HasOne("CarWashBackend.Models.Vehiculo", null)
                        .WithMany()
                        .HasForeignKey("vehiculo_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_vehiculo");
                });

            modelBuilder.Entity("empleado_registro_servicio", b =>
                {
                    b.HasOne("CarWashBackend.Models.Empleado", null)
                        .WithMany()
                        .HasForeignKey("empleado_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_empleado_registro_servicio_empleado");

                    b.HasOne("registro_servicio", null)
                        .WithMany()
                        .HasForeignKey("registro_servicio_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_empleado_registro_servicio_registro_servicio");
                });

            modelBuilder.Entity("registro_servicio", b =>
                {
                    b.HasOne("Cierre", "Cierre")
                        .WithMany("RegistroServicios")
                        .HasForeignKey("CierreId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("CarWashBackend.Models.Cliente", "cliente")
                        .WithMany("registro_servicios")
                        .HasForeignKey("cliente_id")
                        .IsRequired()
                        .HasConstraintName("registro_servicio_ibfk_1");

                    b.HasOne("CarWashBackend.Models.EstadosServicio", "estado_servicio")
                        .WithMany("registro_servicios")
                        .HasForeignKey("estado_servicio_id")
                        .OnDelete(DeleteBehavior.SetNull)
                        .HasConstraintName("fk_estado_servicio");

                    b.Navigation("Cierre");

                    b.Navigation("cliente");

                    b.Navigation("estado_servicio");
                });

            modelBuilder.Entity("registro_servicio_detalle_producto", b =>
                {
                    b.HasOne("Producto", "Producto")
                        .WithMany("RegistroServicioDetalleProductos")
                        .HasForeignKey("ProductoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CarWashBackend.Models.registro_servicio_detalle", "RegistroServicioDetalle")
                        .WithMany("RegistroServicioDetalleProductos")
                        .HasForeignKey("RegistroServicioDetalleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Producto");

                    b.Navigation("RegistroServicioDetalle");
                });

            modelBuilder.Entity("servicio_producto", b =>
                {
                    b.HasOne("Producto", null)
                        .WithMany()
                        .HasForeignKey("producto_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_servicio_producto_producto");

                    b.HasOne("CarWashBackend.Models.Servicio", null)
                        .WithMany()
                        .HasForeignKey("servicio_id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_servicio_producto_servicio");
                });

            modelBuilder.Entity("CarWashBackend.Models.Cliente", b =>
                {
                    b.Navigation("registro_servicios");
                });

            modelBuilder.Entity("CarWashBackend.Models.Empleado", b =>
                {
                    b.Navigation("Usuarios");
                });

            modelBuilder.Entity("CarWashBackend.Models.EstadosServicio", b =>
                {
                    b.Navigation("registro_servicios");
                });

            modelBuilder.Entity("CarWashBackend.Models.Role", b =>
                {
                    b.Navigation("Usuarios");
                });

            modelBuilder.Entity("CarWashBackend.Models.Servicio", b =>
                {
                    b.Navigation("registro_servicio_detalles");
                });

            modelBuilder.Entity("CarWashBackend.Models.Vehiculo", b =>
                {
                    b.Navigation("registro_servicio_vehiculos");
                });

            modelBuilder.Entity("CarWashBackend.Models.registro_servicio_detalle", b =>
                {
                    b.Navigation("RegistroServicioDetalleProductos");
                });

            modelBuilder.Entity("CarWashBackend.Models.registro_servicio_vehiculo", b =>
                {
                    b.Navigation("registro_servicio_detalles");
                });

            modelBuilder.Entity("Cierre", b =>
                {
                    b.Navigation("CierreDetalles");

                    b.Navigation("RegistroServicios");
                });

            modelBuilder.Entity("Producto", b =>
                {
                    b.Navigation("RegistroServicioDetalleProductos");
                });

            modelBuilder.Entity("registro_servicio", b =>
                {
                    b.Navigation("pagos");

                    b.Navigation("registro_servicio_vehiculos");
                });
#pragma warning restore 612, 618
        }
    }
}
