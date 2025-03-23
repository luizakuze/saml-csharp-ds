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
    opt.DefaultChallengeScheme = Saml2Defaults.Scheme; 
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    // // Forçar modo compatível com ambiente de testes HTTP
    // options.Cookie.SameSite = (SameSiteMode)(-1); // Unspecified
    // options.Cookie.SecurePolicy = CookieSecurePolicy.None;

    // // Compatibilidade extra para navegadores antigos
    // options.Events = new CookieAuthenticationEvents
    // {
    //     OnSigningIn = context =>
    //     {
    //         context.CookieOptions.SameSite = (SameSiteMode)(-1);
    //         context.CookieOptions.Secure = false;
    //         return Task.CompletedTask;
    //     }
    // };
})
    .AddSaml2(opt =>
    {
        // Configura as opções do Provedor de Serviço (SP)
        opt.SPOptions.EntityId = new EntityId("https://localhost:5001/Saml2");
        // Adiciona um Provedor de Identidade (IdP)
        opt.IdentityProviders.Add(new IdentityProvider(
            new EntityId("https://stubidp.sustainsys.com/Metadata"), opt.SPOptions)
        {
            LoadMetadata = true, // Carrega os metadados do IdP
            SingleSignOnServiceUrl = new Uri("https://stubidp.sustainsys.com"), // URL do serviço de SSO
            Binding = Sustainsys.Saml2.WebSso.Saml2BindingType.HttpRedirect // Tipo de binding para redirecionamento HTTP
        });
        opt.SPOptions.EntityId = new EntityId("https://localhost:5001/Saml2"); // Define o EntityId do SP
    

    // Remove qualquer certificado previamente carregado
    //opt.SPOptions.ServiceCertificates.Clear();

    // Carrega os certificados
    // TODO: Verificar info certificado
    var encryptionCert = new X509Certificate2("certificates/newcert.pfx", ""); 
    var signingCert = new X509Certificate2("certificates/newcert.pfx", "");

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


    // opt.SPOptions.WantAssertionsSigned = true; // Exigir assinatura das asserções pelo IdP
    // opt.SPOptions.AuthenticateRequestSigningBehavior = SigningBehavior.Always; // SP sempre assina requisições

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
// Adiciona suporte a Razor Pages
builder.Services.AddRazorPages();

// Constrói a aplicação
var app = builder.Build();

// Habilita a execução do pipeline com base no caminho. Isso permite que o resto da aplicação responda tanto em / quanto em /subdir.
app.UsePathBase("/subdir");

// Habilita a autenticação
app.UseAuthentication();

// Configura o roteamento
app.UseRouting();

// Mapeia as Razor Pages
app.MapRazorPages();

// Executa a aplicação
app.Run();