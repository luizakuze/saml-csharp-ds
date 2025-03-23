using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sustainsys.Saml2.AspNetCore2;

namespace samlcsharp.Pages;

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
        // Verifica o valor da propriedade Action e executa a ação correspondente
        return Action switch
        {
            // Se Action for "SignOut", realiza o logout usando os esquemas de autenticação de cookies e SAML2
            "SignOut" => SignOut(CookieAuthenticationDefaults.AuthenticationScheme, Saml2Defaults.Scheme),
            // Se Action for "SignIn", inicia o processo de login
            "SignIn" => Challenge(),
            // Se Action tiver qualquer outro valor, lança uma exceção indicando que a ação não está implementada
            _ => throw new NotImplementedException(),
        };
    }
}