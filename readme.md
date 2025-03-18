# Autenticação com SAML2 em C#
 
Este projeto é uma aplicação ASP.NET Core que implementa um provedor de serviço (SP) que pode se autenticar por meio do protocolo SAML2 utilizando a biblioteca Sustainsys.Saml2 em diferentes provedores de identidades (IDPs) pode meio de um Discovery Service (DS).
 
## Tecnologias Utilizadas 
- ASP.NET Core
- Sustainsys.Saml2 
 
 
## Instalação
1. Clone o repositório:

   ```sh
   git clone https://github.com/luizakuze/saml-csharp 
   ```
2. Instale as dependências:

   ```sh
   dotnet restore saml-csharp.csproj
   ```
3. Execute a aplicação:

   ```sh
   dotnet run saml-csharp.csproj
   ``` 