using System;
using System.Linq;
using CarWashBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace CarWashBackend.Data
{
    public class Seeder
    {
        private readonly CarwashContext _context;
        private readonly bool _resetData;

        public Seeder(CarwashContext context, bool resetData = false)
        {
            _context = context;
            _resetData = resetData;
        }
        public void Seed()
        {
            if (_resetData)
            {
                _context.Permisos.RemoveRange(_context.Permisos);
                _context.Roles.RemoveRange(_context.Roles);
                _context.Usuarios.RemoveRange(_context.Usuarios);
                _context.Empleados.RemoveRange(_context.Empleados);
                _context.SaveChanges();
            }

            if (!_context.Permisos.Any())
            {
                _context.Permisos.AddRange(
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "editar_Usuario", descripcion = "Permiso para ver usuarios", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "Actualizar_vehiculo", descripcion = "Permiso para actualizar un vehiculo", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "Editar Clientes", descripcion = "Permiso para editar clientes", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "ver_permisos", descripcion = "Permiso para ver los permisos", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "Ver Clientes", descripcion = "Permiso para ver clientes", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "Crear vehiculo", descripcion = "Permiso para crear un vehiculo", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "crear_rol", descripcion = "Permiso para crear roles", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "Acceso_Reporte_Financiero", descripcion = "Permiso para acceder a los reportes financieros del sistema", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "crear_empleados", descripcion = "Permiso para crear los empleados", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "ver_empleados", descripcion = "Permiso para ver los empleados", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "crear_usuario", descripcion = "Permiso para ver usuarios", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "Crear Clientes", descripcion = "Permiso para crear clientes", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "Ver_usuarios", descripcion = "Permiso para ver usuarios", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "editar_rol", descripcion = "Permiso para editar roles", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "ver_servicios", descripcion = "Permiso para ver los Servicios", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "Editar_servicio", descripcion = "Permiso para editar los Servicios", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "Ver_registros", descripcion = "Permiso para editar los registros de servicio", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "ver_Registro", descripcion = "Permiso para ver un registro de servicio", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true },
                    new Permiso { id = Guid.NewGuid().ToString(), nombre = "Crear_Servicio", descripcion = "Permiso para crear los Servicios", created_at = DateTime.UtcNow, updated_at = DateTime.UtcNow, activo = true }
                );
                _context.SaveChanges();
            }

            var adminRole = _context.Roles.FirstOrDefault(r => r.nombre == "Administrador");
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    id = Guid.NewGuid().ToString(),
                    nombre = "Administrador",
                    descripcion = "Rol con acceso total al sistema",
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow,
                    activo = true
                };

                _context.Roles.Add(adminRole);
                _context.SaveChanges();
            }

            // Asignar todos los permisos al rol de Administrador si aún no están asignados
            var allPermisos = _context.Permisos.ToList();
            if (!adminRole.permisos.Any())
            {
                adminRole.permisos = allPermisos;
                _context.SaveChanges();
            }

            var empleado = _context.Empleados.FirstOrDefault(e => e.correo == "efrain.aguirre@gmail.com");
            if (empleado == null)
            {
                empleado = new Empleado
                {
                    id = Guid.NewGuid().ToString(),
                    nombre = "Jose Efrain",
                    apellido = "Aguirre",
                    activo = true,
                    correo = "efrain.aguirre@gmail.com",
                    edad = 50,
                    genero = "Masculino",
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow
                };

                _context.Empleados.Add(empleado);
                _context.SaveChanges();
            }

            if (!_context.Usuarios.Any(u => u.usuario1 == "Efrain.Aguirre"))
            {
                var usuario = new Usuario
                {
                    id = Guid.NewGuid().ToString(),
                    usuario1 = "Efrain.Aguirre",
                    contrasena = "admin", // Contraseña en texto plano antes de encriptar
                    empleado_id = empleado.id,
                    role_id = adminRole.id,
                    created_at = DateTime.UtcNow,
                    updated_at = DateTime.UtcNow,
                    activo = true
                };

                // Encriptar la contraseña antes de guardarla
                usuario.contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.contrasena);

                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
            }
        }
    }
}
