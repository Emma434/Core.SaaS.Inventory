using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Core.SaaS.Inventory.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("mock-token/{tenantId}")]
        public IActionResult GetMockToken(Guid tenantId)
        {
            var secretKey = "LaboratorioDev_SuperClaveSecreta_ParaDesarrollo_2026_!#";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Aquí está el corazón de tu aislamiento: inyectamos el TenantId en el pasaporte del usuario
            var claims = new[]
            {
                new Claim("tenant_id", tenantId.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, "admin_local_test")
            };

            var token = new JwtSecurityToken(
                issuer: "Core.SaaS.Inventory.Local",
                audience: "Core.SaaS.Inventory.Local",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // La llave caduca en 2 horas
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}