using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace saml_csharp.Controllers  
{
    [Authorize]
    [Route("secure")]
    public class SecureController : Controller
    {
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var name = User.FindFirst(ClaimTypes.Name)?.Value ?? "Desconhecido";
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "Não informado";

            return Ok(new
            {
                Message = "Usuário autenticado com sucesso!",
                Name = name,
                Email = email
            });
        }
    }
}
