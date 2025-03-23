using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2; // <- PARA .NET 6+
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata; 
using System.Security.Cryptography.X509Certificates;

// Cria o builder da aplicação web
var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args); 

// Configura os serviços de autenticação
builder.Services.AddAuthentication(opt =>
{
    // Define o esquema de autenticação padrão como cookies (mantém sessão do usuário).
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = Saml2Defaults.Scheme; 
})
.AddCookie(opt =>
{
    opt.Cookie.SameSite = SameSiteMode.None;
    opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
})
.AddSaml2(opt =>
    {
        // Configura as opções do Provedor de Serviço (SP)
        opt.SPOptions.EntityId = new EntityId("https://sp-csharp-local:5001/Saml2");
        // Adiciona um Provedor de Identidade (IdP)
        opt.IdentityProviders.Add(new IdentityProvider(
            new EntityId("https://idp2.cafeexpresso.rnp.br/idp/shibboleth"), opt.SPOptions)
        {
            LoadMetadata = true, // Carrega os metadados do IdP
            SingleSignOnServiceUrl = new Uri("https://idp2.cafeexpresso.rnp.br/idp/profile/SAML2/Redirect/SSO"), // URL de SSO
            //SingleLogoutServiceUrl = new Uri("https://idp2.cafeexpresso.rnp.br/idp/profile/SAML2/Redirect/SLO"), // URL de SLO
            Binding = Sustainsys.Saml2.WebSso.Saml2BindingType.HttpRedirect // Tipo de binding para redirecionamento HTTP
        });
    

    // Remove qualquer certificado previamente carregado
    //opt.SPOptions.ServiceCertificates.Clear();

    // Carrega os certificados
    // TODO: Verificar info certificado
    var encryptionCert = new X509Certificate2("Certificates/newcert.pfx", ""); 
    var signingCert = new X509Certificate2("Certificates/newcert.pfx", "");

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


    opt.SPOptions.WantAssertionsSigned = true; // Exigir assinatura das asserções pelo IdP
    opt.SPOptions.AuthenticateRequestSigningBehavior = SigningBehavior.Always; // SP sempre assina requisições

    // Define a URL do Discovery Service, que é usado para selecionar um Identity Provider (IdP) confiável.
    // O Discovery Service permite que os usuários escolham com qual provedor de identidade desejam autenticar-se.
    //opt.SPOptions.DiscoveryServiceUrl = new Uri("https://ds.cafeexpresso.rnp.br/WAYF.php"); 

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
  
});
// Adiciona suporte a Razor Pages
builder.Services.AddRazorPages();

// Constrói a aplicação
var app = builder.Build();

// Habilita a execução do pipeline com base no caminho. Isso permite que o resto da aplicação responda tanto em / quanto em /subdir.
//app.UsePathBase("/subdir");

// Configura o roteamento
app.UseRouting();

app.UseCookiePolicy();

// Habilita a autenticação
app.UseAuthentication();

app.UseAuthorization();

// Mapeia as Razor Pages
app.MapRazorPages();

// Executa a aplicação
app.Run();