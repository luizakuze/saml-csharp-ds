# Autenticação com SAML2 em C#
 
Este projeto é uma aplicação ASP.NET Core que implementa um provedor de serviço (SP) que pode se autenticar por meio do protocolo SAML2 utilizando a biblioteca Sustainsys.Saml2 em um provedor de identidade (IDP).
 
## Tecnologias Utilizadas 
- ASP.NET Core
- Sustainsys.Saml2 
 
## Estrutura do Projeto

```bash
.
├── appsettings.json               # Configurações gerais da aplicação, como parâmetros de inicialização e configuração de serviços.
├── certificates                   # Contém certificados de segurança (como .crt, .pfx e .key) para testes.
│   ├── mycert.crt                  
│   ├── mycert.key                 
│   ├── mycert.pfx                  
│   └── newcert.pfx                 
├── metadata-sp.xml                # Arquivo de metadados do Service Provider (SP)
├── Pages                          # Contém as páginas Razor, que são usadas para gerar conteúdo HTML dinâmico.
│   ├── Index.cshtml               
│   └── Index.cshtml.cs            
├── Program.cs                     # Configura a autenticação SAML2 na aplicação ASP.NET Core
├── Properties                     # Contém configurações adicionais, como informações sobre o ambiente de execução.
│   └── launchSettings.json        
├── readme.md                       
├── saml-csharp.csproj             # Dependências. 
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
 