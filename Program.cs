using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2; // <- PARA .NET 6+
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using System.Security.Cryptography.X509Certificates;

internal class Program
{
    private static void Main(string[] args)
    {
        // Cria o builder da aplicação web
        var builder = WebApplication.CreateBuilder(args);

        // Configurações do appsettings.json
        var config = builder.Configuration;
        var spFqdn = config["Saml:Sp:Fqdn"];
        var spPort = config["Saml:Sp:Port"];
        var idpDomain = config["Saml:Idp:Fqdn"];
        var certDir = config["Saml:Sp:CertDir"];

        var spUrl = $"https://{spFqdn}:{spPort}";

        // Configura os serviços de autenticação
        builder.Services.AddAuthentication(opt =>
        {
            // Define o esquema de autenticação padrão como cookies (mantém sessão do usuário)
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
                opt.SPOptions.EntityId = new EntityId($"{spUrl}/Saml2");

                opt.IdentityProviders.Add(new IdentityProvider(
                    new EntityId("https://idp2.cafeexpresso.rnp.br/idp/shibboleth"), opt.SPOptions)
                {
                    LoadMetadata = true,
                    SingleSignOnServiceUrl = new Uri("https://idp2.cafeexpresso.rnp.br/idp/profile/SAML2/Redirect/SSO"),
                    //SingleLogoutServiceUrl = new Uri($"{idpUrl}/idp/profile/SAML2/Redirect/SLO"),
                    Binding = Sustainsys.Saml2.WebSso.Saml2BindingType.HttpRedirect
                });



                // Configuração de certificados
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
                    Status = CertificateStatus.Current,
                    MetadataPublishOverride = MetadataPublishOverrideType.PublishSigning
                });

                opt.SPOptions.WantAssertionsSigned = true; // Exigir assinatura das asserções pelo IdP
                opt.SPOptions.AuthenticateRequestSigningBehavior = SigningBehavior.Always; // SP sempre assina requisições

                // Define a URL do Discovery Service, que é usado para selecionar um Identity Provider (IdP) confiável 
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

        // Controllers + Views (MVC)
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Configura o roteamento (controllers)
        app.UseRouting();

        // SameSite, Secure, etc.
        app.UseCookiePolicy();

        // Habilita middleware para autenticação e autorização
        app.UseAuthentication();
        app.UseAuthorization();

        // Utilizando controllers
        app.MapDefaultControllerRoute();

        app.Run();
    }
}