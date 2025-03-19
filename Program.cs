using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using System;
using System.Security.Cryptography.X509Certificates;

// Cria o builder da aplicação web
var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Configura os serviços de autenticação
builder.Services.AddAuthentication(opt =>
{
    // Define o esquema de autenticação padrão como cookies (mantém sessão do usuário).
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    // Define o esquema de autenticação para desafios (quando um usuário precisa fazer login) como SAML2.
    opt.DefaultChallengeScheme = Saml2Defaults.Scheme;
})
.AddCookie() // Adiciona autenticação baseada em cookies para manter sessões de usuários autenticados.
.AddSaml2(opt =>
{
    // Define a EntityId do Service Provider (SP), que representa a identidade do nosso sistema dentro do SAML2.
    // Esta URL será usada para obter os metadados da aplicação SAML2.
    // A URL "https://localhost:5001/Saml2" é usada para expor os metadados do SP.
    opt.SPOptions.EntityId = new EntityId("https://localhost:5001/Saml2");

    // Adiciona um certificado digital para assinar mensagens de logout (padrão na doc Sustainsys.Saml2).
    // O certificado precisa ser um arquivo PFX. 
    opt.SPOptions.ServiceCertificates.Add(new X509Certificate2("certificates/newcert.pfx", "")); // Criei também um pfx com senha "mycert.pfx", somente para teste

    // Define a URL do Discovery Service, que é usado para selecionar um Identity Provider (IdP) confiável.
    // O Discovery Service permite que os usuários escolham com qual provedor de identidade desejam autenticar-se.
    // Falta a relação de confiança!!!!
    opt.SPOptions.DiscoveryServiceUrl = new Uri("https://ds.cafeexpresso.rnp.br/WAYF.php"); 
 
    // Exige que asserções SAML enviadas pelo IdP sejam assinadas digitalmente para garantir sua integridade e autenticidade.
    opt.SPOptions.WantAssertionsSigned = true;

    // Faz com que o SP assine todas as solicitações de autenticação, independentemente do que o IdP exige.
    // Isso garante que todas as solicitações de autenticação sejam assinadas.
    opt.SPOptions.AuthenticateRequestSigningBehavior = SigningBehavior.Always; 

});

// Adiciona suporte a Razor Pages (para renderizar páginas HTML dinâmicas no servidor).
builder.Services.AddRazorPages();

// Adiciona suporte a controllers para permitir a utilização de endpoints API e garantir que o SAML2 funcione corretamente.
builder.Services.AddControllers();

// Constrói a aplicação, preparando-a para execução.
var app = builder.Build();

// Adiciona o middleware de autenticação para processar automaticamente requisições autenticadas.
app.UseAuthentication();

// Configura o roteamento, permitindo que a aplicação direcione corretamente as requisições aos endpoints corretos.
app.UseRouting();

// Adiciona a autorização, garantindo que apenas usuários autenticados possam acessar determinados recursos.
app.UseAuthorization();

// Configura os endpoints da aplicação:
// - Razor Pages para servir páginas HTML dinâmicas.
// - Controllers para permitir endpoints API, incluindo suporte para autenticação SAML2.
app.MapRazorPages();
app.MapControllers();

// Obtém e exibe os endpoints registrados no console para depuração.
var routeEndpoints = app.Services.GetRequiredService<Microsoft.AspNetCore.Routing.EndpointDataSource>();
foreach (var endpoint in routeEndpoints.Endpoints)
{
    Console.WriteLine(endpoint.DisplayName);
}

// Inicia a aplicação e começa a escutar requisições HTTP.
app.Run();
