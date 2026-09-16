# 🎮 ArenaSync API

API REST desenvolvida em **ASP.NET Core .NET 8** para gerenciamento de times de e-sports, jogadores e perfis competitivos.

Projeto desenvolvido para o **CP4 - Advanced Business Development with .NET - FIAP 2026**.

---

## 👨‍🎓 Informações Acadêmicas

> **Checkpoint 2 — FIAP**
>
> **Turma:** 2TDSPO
>
> | Aluno                             | RM     |
> |-----------------------------------|--------|
> | Gustavo Gomes Martins             | 555999 |
> | Matheus de Mattos Vecchi          | 561716 |
> | Nicholas Albuquerque Buzo         | 561082 |
> | Nicholas Camillo Canadas de Paula | 561262 |

---

## 🛠️ Tecnologias

- .NET 8
- ASP.NET Core Web API
- C#
- Entity Framework Core 8
- Oracle Database
- Swagger / OpenAPI
- Repository Pattern
- Dependency Injection
- xUnit
- Moq
- Application Insights
- Azure Log Analytics
- Health Checks
- Rate Limiting
- Response Compression

---

# 🏗️ Arquitetura

A aplicação foi organizada em camadas para separar responsabilidades e reduzir o acoplamento.

```text
CP4.sln
│
├── CP4/
│   ├── Controllers/
│   ├── Program.cs
│   └── appsettings.json
│
├── CP4.Domain/
│   └── Entities/
│       ├── Time.cs
│       ├── Jogador.cs
│       └── PerfilCompetitivo.cs
│
├── CP4.Application/
│   ├── Common/
│   ├── DTOs/
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   └── Services/
│   ├── Mappings/
│   ├── Results/
│   └── Services/
│
├── CP4.Infrastructure/
│   ├── Data/
│   │   ├── ApplicationContext.cs
│   │   └── Migrations/
│   └── Repositories/
│
├── CP4.UnitTests/
│
└── CP4.IntegrationTests/
```

## Domain

Contém as entidades e regras estruturais do domínio:

- `Time`
- `Jogador`
- `PerfilCompetitivo`

A camada Domain não depende das demais camadas.

## Application

Responsável pelos casos de uso e regras de negócio.

Contém:

- DTOs
- Services
- Interfaces de repositories
- Interfaces de services
- Mappers
- Result Objects
- Paginação

## Infrastructure

Responsável pelo acesso a recursos externos.

Neste projeto contém:

- Entity Framework Core
- Oracle Database
- `ApplicationContext`
- Implementações dos repositories
- Migrations

## API / Presentation

Responsável pela comunicação HTTP.

Os Controllers recebem as requisições e delegam as regras de negócio para a camada Application.

---

# 🔄 Fluxo da aplicação

```text
HTTP Request
     │
     ▼
Controller
     │
     ▼
Service
     │
     ▼
Repository
     │
     ▼
Entity Framework Core
     │
     ▼
Oracle Database
```

Os Controllers não acessam diretamente o `DbContext`.

---

# 🗄️ Modelo de domínio

## Time

```text
Time
├── Id
├── Nome
├── Jogo
├── Pais
└── Ranking
```

## Jogador

```text
Jogador
├── Id
├── Nickname
├── Funcao
├── Idade
└── TimeId
```

## PerfilCompetitivo

```text
PerfilCompetitivo
├── Id
├── KDA
├── WinRate
├── HorasJogadas
└── JogadorId
```

---

# 🔗 Relacionamentos

## Time 1:N Jogador

Um time pode possuir vários jogadores.

```text
Time
 ├── Jogador
 ├── Jogador
 └── Jogador
```

## Jogador 1:1 PerfilCompetitivo

Cada jogador pode possuir somente um perfil competitivo.

```text
Jogador
   │
   └── PerfilCompetitivo
```

A aplicação também possui uma regra de negócio que impede a criação de dois perfis competitivos para o mesmo jogador.

---

# 📦 DTOs e Mapeamento

A API não utiliza diretamente as entidades do domínio como contrato HTTP.

São utilizados DTOs de entrada:

```text
TimeCreateDto
JogadorCreateDto
PerfilCompetitivoCreateDto
```

E DTOs de resposta:

```text
TimeResumoDto
JogadorResumoDto
PerfilCompetitivoResponseDto
```

A conversão entre entidades e DTOs é realizada pelas classes:

```text
TimeMapper
JogadorMapper
PerfilCompetitivoMapper
```

---

# 📚 Repository Pattern

O acesso aos dados foi abstraído por interfaces na camada Application.

Exemplos:

```text
ITimeRepository
IJogadorRepository
IPerfilCompetitivoRepository
```

As implementações ficam na camada Infrastructure:

```text
TimeRepository
JogadorRepository
PerfilCompetitivoRepository
```

Isso evita que Controllers e Services dependam diretamente do Entity Framework Core.

---

# 📄 Paginação

Os endpoints que retornam coleções possuem paginação utilizando:

```text
pageNumber
pageSize
```

Valores padrão:

```text
pageNumber = 1
pageSize = 10
```

O tamanho máximo de página é:

```text
100 registros
```

Exemplo:

```http
GET /api/Time?pageNumber=1&pageSize=10
```

Resposta:

```json
{
  "items": [
    {
      "id": 1,
      "nome": "FURIA",
      "jogo": "CS2",
      "pais": "Brasil",
      "ranking": 3
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalItems": 1,
  "totalPages": 1
}
```

A paginação é realizada no banco utilizando:

```text
COUNT
ORDER BY
SKIP
TAKE
```

evitando carregar todos os registros para a memória da aplicação.

---

# ⚡ Índices de banco de dados

Foram adicionados índices para campos utilizados frequentemente como filtros.

```text
IX_Times_Jogo
IX_Jogadores_Funcao
```

Também existem índices gerados pelos relacionamentos:

```text
Jogadores.TimeId
PerfilCompetitivo.JogadorId
```

O índice de `PerfilCompetitivo.JogadorId` é único devido ao relacionamento 1:1.

---

# 🚦 Rate Limiting

A API possui Rate Limiting global baseado no endereço IP do cliente.

Configuração atual:

```text
20 requisições
por IP
por minuto
```

Ao ultrapassar o limite:

```http
HTTP 429 Too Many Requests
```

Exemplo de teste:

```bash
for i in {1..21}; do
  curl -s -o /dev/null \
    -w "Requisicao $i: %{http_code}\n" \
    "http://localhost:5105/api/Time?pageNumber=1&pageSize=10"
done
```

A 21ª requisição deve retornar:

```text
429
```

---

# 🗜️ Response Compression

A API possui compressão de respostas HTTP habilitada.

Exemplo:

```bash
curl -s -D - -o /dev/null \
  -H "Accept-Encoding: gzip" \
  "http://localhost:5105/api/Time?pageNumber=1&pageSize=10"
```

O servidor pode responder:

```text
Content-Encoding: gzip
Vary: Accept-Encoding
```

Reduzindo o tamanho transferido das respostas.

---

# ❤️ Health Check

A API expõe:

```http
GET /health
```

O Health Check verifica:

- disponibilidade da API;
- acesso ao Oracle Database;
- tempo de execução de cada verificação.

Exemplo:

```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "api",
      "status": "Healthy",
      "description": "API disponível",
      "durationMs": 0.3
    },
    {
      "name": "oracle-database",
      "status": "Healthy",
      "description": null,
      "durationMs": 11.29
    }
  ],
  "totalDurationMs": 13.55
}
```

Caso uma dependência crítica não esteja disponível, o Health Check pode retornar:

```http
503 Service Unavailable
```

---

# 📝 Logging estruturado

Os Services utilizam `ILogger<T>` para registrar eventos relevantes da aplicação.

Exemplo:

```csharp
_logger.LogWarning(
    "Tentativa de excluir time inexistente {TimeId}",
    id);
```

`TimeId` é mantido como uma propriedade estruturada do log.

São registrados eventos como:

- criação;
- atualização;
- exclusão;
- tentativa de acessar recursos inexistentes;
- violações de regras de negócio.

---

# 📊 Application Insights

A aplicação possui integração com **Azure Application Insights** e **Log Analytics**.

Recursos utilizados:

```text
Resource Group:
rg-arenasync-cp4

Log Analytics Workspace:
law-arenasync-cp4

Application Insights:
arenasync-appinsights
```

A telemetria permite observar:

- requisições HTTP;
- status HTTP;
- duração das requisições;
- traces;
- logs estruturados;
- dependências;
- operações do Entity Framework Core;
- métricas da aplicação.

A Connection String não deve ser armazenada diretamente no código.

No Git Bash:

```bash
export APPLICATIONINSIGHTS_CONNECTION_STRING="$(az monitor app-insights component show \
  --app arenasync-appinsights \
  --resource-group rg-arenasync-cp4 \
  --query connectionString \
  -o tsv)"
```

A aplicação utiliza:

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

O Application Insights é desabilitado no ambiente `Testing` para que os testes automatizados não dependam de serviços externos.

## Exemplo de consulta no Log Analytics

Requests:

```bash
az monitor log-analytics query \
  --workspace "$WORKSPACE_CUSTOMER_ID" \
  --analytics-query "AppRequests | where TimeGenerated > ago(30m) | order by TimeGenerated desc | take 20" \
  -o table
```

Logs:

```bash
az monitor log-analytics query \
  --workspace "$WORKSPACE_CUSTOMER_ID" \
  --analytics-query "AppTraces | where TimeGenerated > ago(30m) | order by TimeGenerated desc | take 30" \
  -o table
```

---

# 🧪 Testes automatizados

O projeto possui testes unitários e testes de integração.

## Testes unitários

Projeto:

```text
CP4.UnitTests
```

São utilizados:

- xUnit
- Moq

Os testes validam regras reais de negócio, como:

- impedir criação de jogador para um time inexistente;
- criar jogador quando o time existe;
- não excluir jogador inexistente;
- impedir perfil para jogador inexistente;
- impedir dois perfis competitivos para o mesmo jogador;
- criar perfil competitivo válido.

Total:

```text
6 testes unitários
```

## Testes de integração

Projeto:

```text
CP4.IntegrationTests
```

Utiliza:

- `WebApplicationFactory`
- Entity Framework Core InMemory

São testados fluxos HTTP reais da API.

Exemplos:

```text
GET /api/Time
GET /api/Time/{id}
GET /health
```

Total:

```text
3 testes de integração
```

## Executar todos os testes

```bash
dotnet test CP4.sln
```

Resultado atual:

```text
6 testes unitários
3 testes de integração

9 testes aprovados
```

---

# 🗄️ Configuração do Oracle

A connection string é configurada através de:

```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=USUARIO;Password=SENHA;Data Source=SERVIDOR:1521/orcl"
  }
}
```

---

# 🔄 Migrations

As migrations estão na camada Infrastructure:

```text
CP4.Infrastructure/Data/Migrations
```

Para aplicar as migrations:

```bash
dotnet ef database update \
  --project CP4.Infrastructure/CP4.Infrastructure.csproj \
  --startup-project CP4/CP4.csproj
```

---

# 🚀 Executando o projeto

## Restaurar dependências

```bash
dotnet restore
```

## Compilar

```bash
dotnet build CP4.sln
```

## Aplicar migrations

```bash
dotnet ef database update \
  --project CP4.Infrastructure/CP4.Infrastructure.csproj \
  --startup-project CP4/CP4.csproj
```

## Executar

```bash
dotnet run --project CP4/CP4.csproj
```

---

# 📄 Swagger / OpenAPI

A API possui documentação utilizando Swagger/OpenAPI.

O Swagger apresenta:

- endpoints;
- parâmetros;
- DTOs;
- códigos de resposta;
- documentação XML;
- descrições das operações.

Em ambiente Development:

```text
http://localhost:PORTA/swagger
```

---

# 🔌 Endpoints

## Time

| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/api/Time?pageNumber=1&pageSize=10` | Lista times paginados |
| GET | `/api/Time/{id}` | Busca time por ID |
| GET | `/api/Time/jogo/{jogo}?pageNumber=1&pageSize=10` | Busca times pelo jogo |
| POST | `/api/Time` | Cria um time |
| PUT | `/api/Time/{id}` | Atualiza um time |
| DELETE | `/api/Time/{id}` | Exclui um time |

## Jogador

| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/api/Jogador?pageNumber=1&pageSize=10` | Lista jogadores paginados |
| GET | `/api/Jogador/{id}` | Busca jogador por ID |
| GET | `/api/Jogador/time/{timeId}?pageNumber=1&pageSize=10` | Filtra jogadores por time |
| GET | `/api/Jogador/funcao/{funcao}?pageNumber=1&pageSize=10` | Filtra jogadores por função |
| POST | `/api/Jogador` | Cria um jogador |
| PUT | `/api/Jogador/{id}` | Atualiza um jogador |
| DELETE | `/api/Jogador/{id}` | Exclui um jogador |

## Perfil Competitivo

| Método | Endpoint | Descrição |
|---|---|---|
| GET | `/api/PerfilCompetitivo?pageNumber=1&pageSize=10` | Lista perfis paginados |
| GET | `/api/PerfilCompetitivo/{id}` | Busca perfil por ID |
| GET | `/api/PerfilCompetitivo/jogador/{jogadorId}` | Busca perfil de um jogador |
| POST | `/api/PerfilCompetitivo` | Cria perfil competitivo |
| PUT | `/api/PerfilCompetitivo/{id}` | Atualiza perfil |
| DELETE | `/api/PerfilCompetitivo/{id}` | Exclui perfil |

---

# 📤 Exemplos de entrada

## Time

```json
{
  "nome": "FURIA",
  "jogo": "CS2",
  "pais": "Brasil",
  "ranking": 3
}
```

## Jogador

```json
{
  "nickname": "KSCERATO",
  "funcao": "Rifler",
  "idade": 25,
  "timeId": 1
}
```

## Perfil competitivo

```json
{
  "kda": 1.32,
  "winRate": 68,
  "horasJogadas": 12000,
  "jogadorId": 1
}
```

---

# ✅ Recursos implementados

- Arquitetura em camadas
- Domain, Application, Infrastructure e Presentation
- Repository Pattern
- Service Layer
- DTO Pattern
- Mappers
- Dependency Injection
- Entity Framework Core
- Oracle Database
- Relacionamentos 1:N e 1:1
- Regras de negócio
- Paginação no banco
- Índices para consultas
- Response Compression
- Rate Limiting por IP
- Swagger / OpenAPI
- XML Documentation
- Health Check da API
- Health Check do Oracle
- Logging estruturado
- Azure Application Insights
- Azure Log Analytics
- Testes unitários
- Testes de integração
- Migrations

---

# ✅ Status

```text
Build                     ✅
Oracle                    ✅
Migrations                ✅
Paginação                 ✅
Índices                   ✅
Response Compression      ✅
Rate Limiting             ✅
Health Check              ✅
Logging estruturado       ✅
Application Insights      ✅
Testes unitários          6/6 ✅
Testes de integração      3/3 ✅
```