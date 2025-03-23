using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sustainsys.Saml2.AspNetCore2;

namespace SamlCsharp.Pages;

[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    [BindProperty]
    public string? Action { get; set; }

    public IActionResult OnPost()
    {
        return Action switch
        {
            "SignOut" => SignOut(
                new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme,
                Saml2Defaults.Scheme),

            "SignIn" => Challenge(new AuthenticationProperties
            {
                RedirectUri = "/Secure" // <- aqui o redirecionamento após login
            }),

            _ => Page()
        };
    }
}
