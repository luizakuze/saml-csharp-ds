using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace SamlCsharp.Controllers;

[IgnoreAntiforgeryToken]
public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index()
    {
        return View(); // Views/Home/Index.cshtml
    }

    [HttpPost("/")]
    public IActionResult Index(string? action)
    {
        return action switch
        {
            "SignIn" => Challenge(new AuthenticationProperties
            {
                RedirectUri = "/users"
            }),
            _ => throw new NotImplementedException(),
        };
    }

    
}
