using CarWashBackend.Models;
using CarWashBackend.Models.NewFolder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarWashBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly CarwashContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(CarwashContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Usuario) || string.IsNullOrEmpty(loginDto.Contrasena))
            {
                return BadRequest("Usuario o contraseña no proporcionados.");
            }

            var usuario = await _context.Usuarios
                .Include(u => u.role)  
                .ThenInclude(r => r.permisos) 
                .FirstOrDefaultAsync(u => u.usuario1 == loginDto.Usuario);

            if (usuario == null)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            // Verificar la contraseña utilizando BCrypt
            bool esContrasenaValida = BCrypt.Net.BCrypt.Verify(loginDto.Contrasena, usuario.contrasena);

            if (!esContrasenaValida)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            // Crear los claims para el JWT
            var claims = new List<Claim>
            {
                new Claim("usuario", usuario.usuario1),
                new Claim("usuarioId", usuario.id)
            };

            // Añadir el rol y los permisos como claims
            claims.Add(new Claim("Rol", usuario.role.nombre)); 

            if (usuario.role.activo)
            {
                foreach (var permiso in usuario.role.permisos)
                {
                    claims.Add(new Claim("Permiso", permiso.nombre));
                }
            }

            // Generación del token JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Retornar el token JWT en la respuesta
            return Ok(new { Token = tokenString });
        }
    }
}
