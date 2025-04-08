using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Configuration;
using Sustainsys.Saml2.Metadata;
using System.Security.Cryptography.X509Certificates;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var config = builder.Configuration;
        var spFqdn = config["Saml:Sp:Fqdn"];
        var spPort = config["Saml:Sp:Port"];
        var spUrl = $"https://{spFqdn}:{spPort}";
        var encryptionCertName = config["Saml:Sp:Certificates:Encryption"];
        var signingCertName = config["Saml:Sp:Certificates:Signing"];

        _ = builder.Services.AddAuthentication(opt =>
        {
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
            // ---------- Configurações do SP ----------
            opt.SPOptions.EntityId = new EntityId($"{spUrl}/Saml2");
            opt.SPOptions.WantAssertionsSigned = true;
            opt.SPOptions.AuthenticateRequestSigningBehavior = SigningBehavior.Always;
            opt.SPOptions.ReturnUrl = new Uri($"{spUrl}/users");
            
            // ---------- Certificados ----------
            var encryptionCert = new X509Certificate2($"Certificates/{encryptionCertName}", "");
            var signingCert = new X509Certificate2($"Certificates/{signingCertName}", "");

            // Certificados para encriptação das asserções
            opt.SPOptions.ServiceCertificates.Add(new ServiceCertificate
            {
                Certificate = encryptionCert,
                Use = CertificateUse.Encryption,
                Status = CertificateStatus.Current,
                MetadataPublishOverride = MetadataPublishOverrideType.PublishEncryption
            });

            // Certificados para assinatura
            opt.SPOptions.ServiceCertificates.Add(new ServiceCertificate
            {
                Certificate = signingCert,
                Use = CertificateUse.Signing,
                Status = CertificateStatus.Current,
                MetadataPublishOverride = MetadataPublishOverrideType.PublishSigning
            });

            // ---------- Discovery Service ----------
            // Redirecionamento para Discovery Service 
            opt.SPOptions.DiscoveryServiceUrl = new Uri("https://ds.cafeexpresso.rnp.br/WAYF.php");
            
            // Carrega metadados da federação 
            new Federation(
                "https://ds.cafeexpresso.rnp.br/metadata/ds-metadata.xml", 
                allowUnsolicitedAuthnResponse: false, 
                opt);

            // foreach (var idp in opt.IdentityProviders.KnownIdentityProviders)
            //     {
            //         Console.WriteLine("🟢 IdP carregado:");
            //         Console.WriteLine($"- EntityID: {idp.EntityId.Id}");
            //         Console.WriteLine($"- SingleSignOnService: {idp.SingleSignOnServiceUrl}");
            //         Console.WriteLine($"- Binding: {idp.Binding.ToString()}");
            //         Console.WriteLine();
            //     }

            // ---------- Metadados de contato ----------
            opt.SPOptions.Contacts.Add(new ContactPerson
            {
                Type = ContactType.Technical,
                Company = "RNP",
                GivenName = "GIdLab",
                Surname = "Equipe",
                EmailAddresses = { "gidlab@rnp.br" }
            });

            // ---------- Metadados de organização ----------
            opt.SPOptions.Organization = new Organization
            {
                Names = { new LocalizedName("GIdLab", "pt-br") },
                DisplayNames = { new LocalizedName("GIdLab", "pt-br") },
                Urls = { new LocalizedUri(new Uri("http://gidlab.rnp.br"), "pt-br") }
            };
        });

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        app.UseRouting();
        app.UseCookiePolicy();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapDefaultControllerRoute();
        app.Run();
    }
}
