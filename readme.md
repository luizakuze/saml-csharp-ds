# Autenticação com SAML2 em C#
 
Esta aplicação ASP.NET Core implementa um provedor de serviço (SP) que autentica usuários via protocolo SAML2, utilizando a biblioteca Sustainsys.Saml2 e um Discovery Service (DS) para conectar-se aos provedores de identidade (IdP) da federação.
 
## Estrutura do Projeto

```bash
.
├── AttributeMap                # Mapeamento de atributos SAML  
│   └── SamlUri.cs              
├── Certificates                 # Certificados para assinatura e encriptação das asserções SAML 
│   ├── mycert.crt               # Certificado público
│   ├── mycert.key               # Chave privada correspondente ao certificado
│   └── newcert.pfx              # Certificado + chave combinados em formato PFX (usado pelo SP)
├── Controllers                  # Lógica dos endpoints da aplicação
│   ├── HomeController.cs        
│   ├── LogoutController.cs       
│   └── UsersController.cs        
├── Views                        # Páginas HTML renderizadas pelos controllers
│   ├── Home
│   │   └── Index.cshtml            
│   └── Users
│       └── Index.cshtml        
├── appsettings.json             # Configurações da aplicação (logs, ambiente e variáveis)
├── saml-csharp.csproj           # Arquivo de configuração do projeto C# (.NET)
├── Program.cs                   # Ponto de entrada da aplicação e configuração do serviço 
├── metadata-sp.xml              # Metadado do SP
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
 

## Preparando ambiente para novo contexto

1. Gerar novos certificados para encriptação e asserções SAML.

   1. Criar um diretório chamado `Certificates`:

      ```bash
      mkdir Certificates 
      ```

   2. Incluir certificados para encriptação e assinatura das asserções SAML:

      ```bash
      # Criar a chave
      openssl genrsa -out Certificates/mykey.key 2048
 
      # Criar o certificado a partir da chave
      openssl req -new -key Certificates/mykey.key -out Certificates/mycert.csr
      openssl x509 -req -days 365 -in Certificates/mycert.csr -signkey Certificates/mykey.key -out Certificates/mycert.crt

      # Gerar o arquivo .pfx (chave + certificado)
      openssl pkcs12 -export \
      -out Certificates/newcert.pfx \
      -inkey Certificates/mykey.key \
      -in Certificates/mycert.crt \
      -certfile Certificates/mycert.crt
      ```
2. Modificações para o contexto como FQDN e portas devem ser feitas no arquivo `appsettings.json`:
      ```bash
         {
         "Logging": {
            "LogLevel": {
               "Default": "Information",
               "Microsoft.AspNetCore": "Warning"
            }
         },
         "AllowedHosts": "*",

         "Saml": {
            "Sp": {
               "Fqdn": "sp-csharp-local", # FQDN do SP
               "Port": "5001",            # Porta do SP
               "CertDir": "Certificates"  # Diretório para certificados
            } 
         }
      ```