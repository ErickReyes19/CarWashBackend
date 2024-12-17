using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWashBackend.Migrations
{
    /// <inheritdoc />
    public partial class SegundaMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Usuarios_ibfk_1",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "RolPermisos");

            migrationBuilder.DropTable(
                name: "UsuarioRoles");

            migrationBuilder.DropIndex(
                name: "nombre1",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "usuario_id",
                table: "RegistroServicios",
                newName: "usuario_id1");

            migrationBuilder.AddColumn<string>(
                name: "role_id",
                table: "Usuarios",
                type: "varchar(36)",
                maxLength: 36,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "usuario_id",
                table: "Empleados",
                type: "varchar(36)",
                maxLength: 36,
                nullable: true,
                collation: "utf8mb4_0900_ai_ci")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolePermisos",
                columns: table => new
                {
                    rol_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    permiso_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.rol_id, x.permiso_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "FK_RolePermisos_Permiso",
                        column: x => x.permiso_id,
                        principalTable: "Permisos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermisos_Rol",
                        column: x => x.rol_id,
                        principalTable: "Role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "FK_Usuario_Role",
                table: "Usuarios",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "usuario_id",
                table: "Empleados",
                column: "usuario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_RolePermisos_Permiso",
                table: "RolePermisos",
                column: "permiso_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Empleado_Usuario",
                table: "Empleados",
                column: "usuario_id",
                principalTable: "Usuarios",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Empleado",
                table: "Usuarios",
                column: "empleado_id",
                principalTable: "Empleados",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Role",
                table: "Usuarios",
                column: "role_id",
                principalTable: "Role",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empleado_Usuario",
                table: "Empleados");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Empleado",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Role",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "RolePermisos");

            migrationBuilder.DropIndex(
                name: "FK_Usuario_Role",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "usuario_id",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "role_id",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "usuario_id",
                table: "Empleados");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "Roles");

            migrationBuilder.RenameIndex(
                name: "usuario_id1",
                table: "RegistroServicios",
                newName: "usuario_id");

            migrationBuilder.CreateTable(
                name: "RolPermisos",
                columns: table => new
                {
                    rol_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    permiso_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.rol_id, x.permiso_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "RolPermisos_ibfk_1",
                        column: x => x.rol_id,
                        principalTable: "Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "RolPermisos_ibfk_2",
                        column: x => x.permiso_id,
                        principalTable: "Permisos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateTable(
                name: "UsuarioRoles",
                columns: table => new
                {
                    usuario_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rol_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false, collation: "utf8mb4_0900_ai_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.usuario_id, x.rol_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "UsuarioRoles_ibfk_1",
                        column: x => x.usuario_id,
                        principalTable: "Usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "UsuarioRoles_ibfk_2",
                        column: x => x.rol_id,
                        principalTable: "Roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "nombre1",
                table: "Roles",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "permiso_id",
                table: "RolPermisos",
                column: "permiso_id");

            migrationBuilder.CreateIndex(
                name: "rol_id",
                table: "UsuarioRoles",
                column: "rol_id");

            migrationBuilder.AddForeignKey(
                name: "Usuarios_ibfk_1",
                table: "Usuarios",
                column: "empleado_id",
                principalTable: "Empleados",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
