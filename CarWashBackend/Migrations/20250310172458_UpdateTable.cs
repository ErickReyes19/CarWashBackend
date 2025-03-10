using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWashBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "descripcion",
                table: "registro_servicio",
                type: "Text",
                nullable: false,
                defaultValueSql: "Descripcion",
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "producto",
                type: "tinyint(1)",
                nullable: true,
                defaultValueSql: "'1'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "descripcion",
                table: "registro_servicio");

            migrationBuilder.DropColumn(
                name: "activo",
                table: "producto");

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
        }
    }
}
