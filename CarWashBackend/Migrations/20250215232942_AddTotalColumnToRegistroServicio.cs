﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWashBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalColumnToRegistroServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "total",
                table: "registro_servicio",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "total",
                table: "registro_servicio");
        }
    }
}
