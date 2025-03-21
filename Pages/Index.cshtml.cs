using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sustainsys.Saml2.AspNetCore2;

namespace SamlCsharp.Pages;

// Ignora o token antifalsificação para esta página
[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    // Método que é chamado quando a página é acessada via GET
    public void OnGet()
    {
    }

    // Propriedade que será vinculada aos dados do formulário enviado
    [BindProperty]
    public string? Action { get; set; }

    // Método que é chamado quando a página é acessada via POST
public IActionResult OnPost()
{
    return Action switch
    {
        "SignOut" => SignOut(CookieAuthenticationDefaults.AuthenticationScheme, Saml2Defaults.Scheme),

        // Inicia corretamente o fluxo SP-Initiated com retorno definido
        "SignIn" => Challenge(new AuthenticationProperties
        {
            RedirectUri = "/"
        }, Saml2Defaults.Scheme),

        _ => throw new NotImplementedException(),
    };
}
}
