# Organizas — API

Backend para gerenciamento de listas de compras pessoais, desenvolvido com ASP.NET Core. Oferece autenticação, perfil de usuário e gerenciamento de listas e itens, com acesso restrito ao proprietário.

## Tecnologias

- .NET 10 e ASP.NET Core.
- Entity Framework Core e PostgreSQL.
- ASP.NET Core Identity.
- FluentValidation.
- MailKit para envio SMTP.
- Swagger/OpenAPI.
- xUnit e SQLite em memória para testes.

## Funcionalidades

- Cadastro, confirmação de e-mail, login e recuperação de senha.
- Criação e atualização do perfil.
- Criação, consulta, edição e exclusão de listas de compras.
- Itens com quantidade, unidade, preço estimado e marcação de comprado.
- Isolamento de listas e itens por usuário.
- Exclusão automática dos itens ao excluir uma lista.

O fluxo principal é:

```text
Cadastro → Confirmação de e-mail → Login → Perfil → Listas e itens
```

**O perfil deve ser criado antes da primeira lista**, pois as listas estão vinculadas a ele.

## Executar localmente

### Pré-requisitos

- SDK do .NET 10.
- PostgreSQL acessível, com credenciais e permissões para aplicar migrations.
- Credenciais SMTP. Para desenvolvimento, pode ser usado o Mailtrap Email Sandbox.

Execute os comandos abaixo na raiz do repositório.

### 1. Restaurar dependências

```powershell
dotnet restore Organizas.slnx
dotnet tool restore
dotnet dev-certs https --trust
```

### 2. Configurar os segredos

O projeto já possui `UserSecretsId`. No Visual Studio, abra **Gerenciar Segredos do Usuário** no projeto `Organizas` e configure:

```json
{
  "ConnectionStrings": {
    "Organizas": "Host=localhost;Port=5432;Database=organizas;Username=SEU_USUARIO;Password=SUA_SENHA"
  },
  "Email": {
    "Host": "sandbox.smtp.mailtrap.io",
    "Port": 587,
    "Username": "USUARIO_SMTP",
    "Password": "SENHA_SMTP",
    "FromAddress": "nao-responda@organizas.example",
    "FromName": "Organizas"
  }
}
```

As seções `Email` e `ConnectionStrings` devem ficar na raiz do JSON. O remetente acima é ilustrativo para testes.

Os User Secrets são carregados automaticamente no ambiente `Development`. Não versione credenciais. Em produção, configure-as pelo ambiente de execução, usando chaves como `ConnectionStrings__Organizas` e `Email__Password`.

O envio utiliza STARTTLS. No Mailtrap Sandbox, as mensagens aparecem na caixa de testes do Mailtrap, sem entrega ao destinatário real. A aplicação valida as configurações SMTP ao iniciar.

### 3. Aplicar migrations e iniciar

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"

dotnet ef database update --project Organizas/Organizas.csproj

dotnet run --project Organizas/Organizas.csproj --launch-profile https
```

- API: `https://localhost:7108`
- [Swagger local](https://localhost:7108/swagger)

Para um ambiente novo, prefira um banco vazio. A migration de vínculo com o perfil exige atenção se já houver listas antigas sem proprietário.

## Autenticação

Os endpoints padrão do Identity estão agrupados em `/Auth`.

| Método | Rota | Finalidade |
| ------ | ---- | ---------- |
| POST | `/Auth/register` | Criar conta |
| GET | `/Auth/confirmEmail` | Confirmar e-mail pelo link recebido |
| POST | `/Auth/resendConfirmationEmail` | Reenviar confirmação |
| POST | `/Auth/login` | Autenticar |
| POST | `/Auth/refresh` | Renovar tokens |
| POST | `/Auth/forgotPassword` | Solicitar recuperação |
| POST | `/Auth/resetPassword` | Redefinir senha |

Para autenticação por token:

```http
POST /Auth/login?useCookies=false
Content-Type: application/json
```

```json
{
  "email": "usuario@example.com",
  "password": "SUA_SENHA"
}
```

Envie o access token nas requisições protegidas:

```http
Authorization: Bearer SEU_ACCESS_TOKEN
```

O Identity também aceita login por cookies com `useCookies=true`. Os bearer tokens emitidos são próprios do Identity, não JWTs.

O login exige e-mail confirmado. A senha possui comprimento mínimo de 10 caracteres, além das demais regras padrão do Identity. Após 5 tentativas inválidas, a conta pode ficar bloqueada por 10 minutos.

## Endpoints da aplicação

Todos os endpoints abaixo exigem autenticação.

| Método | Rota | Finalidade |
| ------ | ---- | ---------- |
| GET | `/UserProfile` | Consultar o próprio perfil |
| PUT | `/UserProfile` | Criar ou atualizar o próprio perfil |
| GET | `/ShoppingList` | Listar as próprias listas |
| GET | `/ShoppingList/{id}` | Consultar uma lista com seus itens |
| POST | `/ShoppingList` | Criar uma lista |
| PUT | `/ShoppingList/{id}` | Atualizar uma lista |
| DELETE | `/ShoppingList/{id}` | Excluir a lista e seus itens |
| POST | `/ShoppingList/{shoppingListId}/items` | Adicionar um item |
| PUT | `/ShoppingList/{shoppingListId}/items/{itemId}` | Atualizar um item |
| DELETE | `/ShoppingList/{shoppingListId}/items/{itemId}` | Excluir um item |

O proprietário é identificado pela autenticação, não pelo corpo da requisição. Recursos inexistentes ou pertencentes a outra conta retornam `404`.

Consulte o Swagger para os campos dos DTOs e valores das unidades de medida.

## Respostas e erros

Os controllers da aplicação utilizam `ApiResponseDto<T>`:

```json
{
  "success": true,
  "data": {
    "name": "Nome"
  },
  "message": "Perfil salvo com sucesso",
  "errors": null,
  "traceId": null
}
```

- `200`: consulta, atualização ou exclusão concluída.
- `201`: lista ou item criado.
- `400`: dados inválidos.
- `401`: autenticação ausente ou inválida.
- `404`: recurso não encontrado ou inacessível.
- `500`: falha inesperada.

Exclusões retornam `200` com mensagem e `data: null`. O handler central inclui erros de validação por campo e `traceId` para diagnóstico.

Os endpoints do Identity e algumas respostas automáticas do ASP.NET Core possuem formatos próprios.

## Organização

```text
Organizas/
├── Controllers/    # Rotas e respostas HTTP
├── Services/       # Operações, validação e acesso aos dados
├── Entities/       # Modelo persistido
├── Dtos/           # Contratos de entrada e saída
├── Validations/    # Regras do FluentValidation
├── Exceptions/     # Exceções da aplicação
├── Infra/          # Banco, e-mail e tratamento de erros
└── Migrations/     # Evolução do banco

Organizas_Tests/    # Testes dos serviços
```

## Testes

```powershell
dotnet test Organizas.slnx
```

A suíte utiliza xUnit e SQLite em memória, sem depender de PostgreSQL ou SMTP externos. Cobre consultas, isolamento entre contas, preservação de dados em operações negadas e exclusão em cascata.

Os testes de serviço não substituem testes HTTP de autenticação nem validação das migrations no PostgreSQL.

## CI/CD

O GitHub Actions está configurado para compilar, testar e gerar os artefatos da API e das migrations em pull requests e pushes para `main`.

Em pushes para `main`, o pipeline também executa a implantação na AWS usando S3, Systems Manager e EC2. Essa etapa depende da infraestrutura e das permissões configuradas para o projeto.

## Integração com o frontend

O frontend é mantido em outro repositório. O backend ainda não configura CORS; para chamadas pelo navegador ou pela WebView do aplicativo, configure explicitamente as origens necessárias ou utilize um proxy apropriado.
