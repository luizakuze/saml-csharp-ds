using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2;
using Sustainsys.Saml2.AspNetCore2;
using Sustainsys.Saml2.Metadata;

// Cria o builder da aplicação web
var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Configura os serviços de autenticação
builder.Services.AddAuthentication(opt =>
    {
        // Define o esquema padrão de autenticação como Cookie
        opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        // Define o esquema de desafio padrão como Saml2
        opt.DefaultChallengeScheme = Saml2Defaults.Scheme;
    })
    .AddCookie() // Adiciona autenticação baseada em Cookie
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
