using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWashBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddProductoAndRelationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "fecha",
                table: "registro_servicio",
                type: "Text",
                nullable: false,
                defaultValueSql: "Descripcion",
                collation: "utf8mb4_0900_ai_ci",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "CURRENT_TIMESTAMP")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "producto",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    descripcion = table.Column<string>(type: "text", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    precio = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "registro_servicio_detalle_productos",
                columns: table => new
                {
                    RegistroServicioDetalleId = table.Column<string>(type: "varchar(50)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductoId = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_registro_servicio_detalle_productos", x => new { x.RegistroServicioDetalleId, x.ProductoId });
                    table.ForeignKey(
                        name: "FK_registro_servicio_detalle_productos_producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_registro_servicio_detalle_productos_registro_servicio_detall~",
                        column: x => x.RegistroServicioDetalleId,
                        principalTable: "registro_servicio_detalle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "servicio_producto",
                columns: table => new
                {
                    servicio_id = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    producto_id = table.Column<string>(type: "varchar(36)", nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicio_producto", x => new { x.servicio_id, x.producto_id });
                    table.ForeignKey(
                        name: "fk_servicio_producto_producto",
                        column: x => x.producto_id,
                        principalTable: "producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_servicio_producto_servicio",
                        column: x => x.servicio_id,
                        principalTable: "Servicios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_registro_servicio_detalle_productos_ProductoId",
                table: "registro_servicio_detalle_productos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_servicio_producto_producto_id",
                table: "servicio_producto",
                column: "producto_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "registro_servicio_detalle_productos");

            migrationBuilder.DropTable(
                name: "servicio_producto");

            migrationBuilder.DropTable(
                name: "producto");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha",
                table: "registro_servicio",
                type: "datetime",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(string),
                oldType: "Text",
                oldDefaultValueSql: "Descripcion")
                .OldAnnotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "utf8mb4_0900_ai_ci");
        }
    }
}
