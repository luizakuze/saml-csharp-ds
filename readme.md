# Autenticação com SAML2 em C#
 
Este projeto é uma aplicação ASP.NET Core que implementa um provedor de serviço (SP) que pode se autenticar por meio do protocolo SAML2 utilizando a biblioteca Sustainsys.Saml2 em um provedor de identidade (IDP).
 
## Tecnologias Utilizadas 
- ASP.NET Core
- Sustainsys.Saml2 
 
## Estrutura do Projeto

```bash
.
├── AttributeMaps
│   ├── AdfsV1xMap.cs
│   ├── AdfsV20Map.cs
│   ├── Basic.cs
│   ├── SamlUri.cs
│   └── ShibbolethUri.cs 
├── Certificates
│   ├── mycert.crt
│   ├── mycert.key
│   └── newcert.pfx
├── Controllers
│   ├── HomeController.cs
│   ├── LogoutController.cs
│   └── UsersController.cs
└── Views
    ├── Home
    │   └── Index.cshtml
    ├── Logout
    │   └── Index.cshtml
    └── Users
        └── Index.cshtml
├── Properties
│   └── launchSettings.json
├── saml-csharp.csproj
├── saml-csharp.sln
├── appsettings.json
├── Program.cs
├── metadata-sp.xml
└── readme.md

```

## Instalação
1. Clone o repositório:

   ```sh
   git clone https://github.com/luizakuze/saml-csharp-idp2
   ```
2. Adicione a seguinte linha ao arquivo  `/etc/hosts`:

   ```sh
   127.0.0.1 sp-csharp-local
   ```
3. Instale as dependências:

   ```sh
   dotnet restore saml-csharp.csproj
   ```
4. Execute a aplicação:

   ```sh
   dotnet run saml-csharp.csproj
   ``` 
 