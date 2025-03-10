using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWashBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cantidad",
                table: "registro_servicio_detalle_productos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cantidad",
                table: "registro_servicio_detalle_productos");
        }
    }
}
