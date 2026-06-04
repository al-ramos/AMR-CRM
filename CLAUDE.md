# AMR-CRM — Contexto para Claude Code

## Identidade do módulo
Módulo CRM do AMR SYSTEM — gestão de contatos, leads, prospects e oportunidades de negócio.
- **API local**: http://localhost:5187/swagger
- **Web local**: http://localhost:5176
- **Banco**: SQLite via EF Core 10 (EFS `/data` em produção)

## Ecossistema AMR SYSTEM
- **AMR-Financeiro** — SQL Server, porta API :5015, web :5173
- **AMR-Core** — SQLite, porta API :5001, web :5175
- **AMR-Fábrica** — SQLite, porta API :5186, web :5174
- **AMR-CRM** (este repo) — SQLite, porta API :5187, web :5176

## Stack
- Backend: .NET 10 + Clean Architecture + CQRS (MediatR 12+)
- ORM: EF Core + SQLite + Migrations
- Frontend: React 19 + TypeScript + Vite 6.0.5 + Bootstrap 5 + Lucide React
- Testes: xUnit + Coverlet + NSubstitute
- Infra: AWS ECS Fargate + ECR + ALB + EFS | CI/CD: GitHub Actions

## Arquitetura
```
src/
├── AMR.CRM.Domain/          # Entidades, enums, interfaces
├── AMR.CRM.Application/     # CQRS handlers, DTOs, queries, commands, validators
├── AMR.CRM.Infrastructure/  # EF Core, SQLite, repositories, UoW, migrations
├── AMR.CRM.Shared/          # Result<T>
└── AMR.CRM.API/             # Controllers, Middleware, Program.cs
frontend/                     # React 19 + Bootstrap 5
tests/
├── AMR.CRM.Domain.Tests/     # testes de domínio
└── AMR.CRM.Application.Tests/ # handler tests com NSubstitute
```

Padrões: Clean Architecture, CQRS+MediatR, Repository Pattern, Unit of Work, DI, ValidationBehavior.

## Entidades do Domínio
- `Contato` — lead/prospect/cliente/parceiro, workflow Ativo ↔ Inativo
- `Lead` — pipeline de vendas: Novo → Qualificado → Proposta → Ganho/Perdido; campos: Nome, Email, Empresa, Telefone, Origem (LinkedIn/Website/Indicação/…), ValorEstimado
- `Oportunidade` — vinculada a Lead (LeadId) ou Contato (ContatoId), Probabilidade %, workflow: Aberta → EmAndamento → Ganha/Perdida/Cancelada

## Comandos Principais
```bash
# Backend
cd src/AMR.CRM.API && dotnet run
# → http://localhost:5187/swagger

# Frontend
cd frontend && npm install && npm run dev
# → http://localhost:5176

# Testes (31/31 passando)
dotnet test

# Migrations
dotnet ef migrations add <Nome> --project src/AMR.CRM.Infrastructure --startup-project src/AMR.CRM.API
dotnet ef database update --project src/AMR.CRM.Infrastructure --startup-project src/AMR.CRM.API
```

## Estado do Projeto — Sprint 7 (04/06/2026)

### ✅ Entregues
| Card | Commit | Data |
|---|---|---|
| 🏗️ Scaffold AMR-CRM (.NET 10 + React 19) | `c51f265` | 04/06/2026 |
| 👤 CRUD Leads + pipeline de status | `17823a5` | 04/06/2026 |

### ▶️ Em andamento
- **💼 CRUD de Oportunidades vinculadas a Leads** — Notion: https://app.notion.com/p/375d35f21de581e5af4bc41bf528ff13

### 🔲 Backlog Sprint 7
- 📊 Dashboard CRM — pipeline Kanban + métricas
- 🔔 Atividades / histórico de interações por Lead
- 🔗 Integração RabbitMQ/MassTransit com AMR-Core (sync clientes)

## Testes — 31/31 passando (04/06/2026)
- 24 domain tests (LeadTests × 10, ContatoTests × 5, OportunidadeTests × 7, herdados × 2)
- 7 application handler tests com NSubstitute (CriarLead, AtualizarLead, AvancarStatus, ExcluirLead)

## Troubleshooting
| Problema | Solução |
|---|---|
| Porta errada no backend | `launchSettings.json` → `applicationUrl: http://localhost:5187` |
| CORS bloqueando frontend | `appsettings.Development.json` → `AllowedOrigins: http://localhost:5176` |
| MediatR não resolve handlers | Verificar `RegisterServicesFromAssembly` no `Program.cs` |
| Vite proxy não funciona | `vite.config.ts` → `target: http://localhost:5187` |
| Vite crash via junction Windows | `launch.json`: usar `npm --prefix C:\GitHub\AMR-CRM\frontend run dev` em vez de `cwd: junction-path` — evita mismatch de path real vs junction no dep optimizer |
| Vite versão | Pinado em `6.0.5` — versões acima crasham no Node 24 via junction |
