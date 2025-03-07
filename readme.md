# WebApplication - Autenticação com SAML2


> Projeto baseado no repositório: [Sustainsys.Saml2](https://github.com/Sustainsys/Saml2)



Este projeto é uma aplicação ASP.NET Core que implementa autenticação baseada no protocolo SAML2 usando a biblioteca Sustainsys.Saml2. Ele permite que os usuários realizem login e logout por meio de um Provedor de Identidade (IdP). 
## Tecnologias Utilizadas
- .NET 8.0
- ASP.NET Core
- Sustainsys.Saml2
- Autenticação baseada em Cookies
- Razor Pages

## Funcionalidades
- Suporte a login via SAML2
- Autenticação baseada em cookies
- Suporte a logout (Single Logout Service - SLO)
- Implementação baseada em Razor Pages
 
## Instalação
1. Clone o repositório:

   ```sh
   git clone https://github.com/luizakuze/saml-csharp 
   ```
2. Instale as dependências:

   ```sh
   dotnet restore WebApplication.csproj
   ```
3. Execute a aplicação:

   ```sh
   dotnet run WebApplication.csproj
   ``` 