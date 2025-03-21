using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2; // <- PARA .NET 6+
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.WebSso;
using System;
using System.Runtime.ConstrainedExecution;
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
.AddCookie()
// .AddCookie(options =>
// {
//     options.Cookie.Name = ".AspNetCore.SamlSession";
//     options.Cookie.HttpOnly = true;
//     options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//     options.Cookie.SameSite = (SameSiteMode)(-1); // Equivalent to "Unspecified"
// })
.AddSaml2(opt =>
{ 
    // Define a EntityId do Service Provider (SP), que representa a identidade do nosso sistema dentro do SAML2.
    // A URL "https://localhost:5001/Saml2" é usada para expor os metadados do SP.
    // Caso seja em produção colocar HTTPS e em ./Properties/launchSettings.json
    opt.SPOptions.EntityId = new EntityId("http://sp-csharp-local:5001/Saml2"); 
 
    // Remove qualquer certificado previamente carregado
    opt.SPOptions.ServiceCertificates.Clear();

    // Carrega os certificados
    // TODO: Verificar info certificado
    var encryptionCert = new X509Certificate2("certificates/newcert.pfx", ""); 
    var signingCert = new X509Certificate2("certificates/newcert.pfx", "");

    opt.SPOptions.WantAssertionsSigned = true; // Exigir assinatura das asserções pelo IdP

    opt.SPOptions.ServiceCertificates.Add(new ServiceCertificate
    {
        Certificate = encryptionCert,
        Use = CertificateUse.Encryption,
        Status = CertificateStatus.Current,
        MetadataPublishOverride = MetadataPublishOverrideType.PublishEncryption
    });

    opt.SPOptions.ServiceCertificates.Add(new ServiceCertificate
    {
        Certificate = signingCert,
        Use = CertificateUse.Signing,
        Status = CertificateStatus.Current, // TODO: verificar se está correto como "Future"
        MetadataPublishOverride = MetadataPublishOverrideType.PublishSigning 
    });

    // TESTE PARA PROVEDOR DE IDENTIDADE
    opt.IdentityProviders.Add(
        new IdentityProvider(
            new EntityId("https://idp2.cafeexpresso.rnp.br/idp/shibboleth"), 
            opt.SPOptions)
        {
            MetadataLocation = "https://idp2.cafeexpresso.rnp.br/idp/shibboleth",
            LoadMetadata = true
        });

    opt.SPOptions.WantAssertionsSigned = true; // Exigir assinatura das asserções pelo IdP
    opt.SPOptions.AuthenticateRequestSigningBehavior = SigningBehavior.Always; // SP sempre assina requisições

    // Define a URL do Discovery Service, que é usado para selecionar um Identity Provider (IdP) confiável.
    // O Discovery Service permite que os usuários escolham com qual provedor de identidade desejam autenticar-se.
    // TODO: Relação de confiança
    opt.SPOptions.DiscoveryServiceUrl = new Uri("https://ds.cafeexpresso.rnp.br/WAYF.php"); 

    // Configuração de retorno... Configuração já está sendo feita no indes.cshtml.cs (????)
    // TODO: Verificar se é necessário adicionar esse atributo 
    //opt.SPOptions.ReturnUrl = new Uri("https://localhost:5001/");

    // Faz com que o SP assine todas as solicitações de autenticação, independentemente do que o IdP exige.
    // Isso garante que todas as solicitações de autenticação sejam assinadas.
    opt.SPOptions.AuthenticateRequestSigningBehavior = SigningBehavior.Always; 
 
    // opt.SPOptions.AttributeConsumingServices.Add(new AttributeConsumingService
    // {
    //     ServiceNames = { new LocalizedName("SP C#", "en") },
    //     ServiceDescriptions = new LocalizedName("Provedor de serviços C#", "en"),
    //     InformationUrl = new LocalizedUri(new Uri("http://sp.information.url/"), "en"),
    //     PrivacyStatementUrl = new LocalizedUri(new Uri("http://sp.privacy.url/"), "en")
    // });

    // Configura os metadados de contato
    opt.SPOptions.Contacts.Add(new ContactPerson
    {
        Type = ContactType.Technical,
        Company = "RNP",
        GivenName = "GIdLab",
        Surname = "Equipe",
        EmailAddresses = { "gidlab@rnp.br" }
    });

    // Configura os metadados de organização
    opt.SPOptions.Organization = new Organization
    {
        Names = { new LocalizedName("GIdLab", "pt-br") },
        DisplayNames = { new LocalizedName("GIdLab", "pt-br") },
        Urls = { new LocalizedUri(new Uri("http://gidlab.rnp.br"), "pt-br") }
    };

    // Define o ModulePath para o SAML2. Isso altera os caminhos base dos endpoints SAML.
    // Isso é útil se você quiser ter um caminho base para todas as rotas SAML2.
    //opt.SPOptions.ModulePath = "/saml2"; // Caminho base para todos os endpoints SAML2 (login, logout, etc.)
 
    //opt.SPOptions.ReturnUrl = new Uri("https://localhost:5001/"); // URL de retorno após a autenticação (página inicial).

    // Adiciona a configuração do AttributeConsumingService
    // opt.SPOptions.AttributeConsumingServices.Add(new AttributeConsumingService
    // {
    //     display_name = new LocalizedName("SP C#", "en"),
    //     Description = new LocalizedName("Provedor de serviços C#", "en"),
    //     InformationUrl = new LocalizedName("http://sp.information.url/", "en"),
    //     PrivacyStatementUrl = new LocalizedName("http://sp.privacy.url/", "en")
    // });
 
});

// Adiciona suporte a Razor Pages (para renderizar páginas HTML dinâmicas no servidor).
builder.Services.AddRazorPages();

// Adiciona suporte a controllers para permitir a utilização de endpoints API e garantir que o SAML2 funcione corretamente.
builder.Services.AddControllers();

// Constrói a aplicação, preparando-a para execução.
var app = builder.Build();
 

// Adiciona o middleware de autenticação para processar automaticamente requisições autenticadas.
app.UseCookiePolicy();
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
