using CarWashBackend.Models;
using CarWashBackend.Models.NewFolder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
                .Include(u => u.rols) 
                .ThenInclude(r => r.permisos) 
                .FirstOrDefaultAsync(u => u.usuario == loginDto.Usuario);

            if (usuario == null)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            if (usuario.contrasena != loginDto.Contrasena)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            var claims = new List<Claim>
            {
                new Claim("usuario", usuario.usuario),
                new Claim("uduarioId", usuario.id),
            };

            foreach (var role in usuario.rols)
            {
                claims.Add(new Claim("Rol", role.nombre));

                foreach (var permiso in role.permisos)
                {
                    claims.Add(new Claim("Permiso", permiso.nombre));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { Token = tokenString });
        }
    }
}



