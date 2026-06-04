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
- Frontend: React 19 + TypeScript + Vite + Bootstrap 5 + Lucide React
- Testes: xUnit + Coverlet
- Infra: AWS ECS Fargate + ECR + ALB + EFS | CI/CD: GitHub Actions

## Arquitetura
```
src/
├── AMR.CRM.Domain/          # Entidades, enums, interfaces
├── AMR.CRM.Application/     # CQRS handlers, DTOs, queries, commands
├── AMR.CRM.Infrastructure/  # EF Core, SQLite, repositories, UoW, migrations
├── AMR.CRM.Shared/          # Result<T>
└── AMR.CRM.API/             # Controllers, Middleware, Program.cs
frontend/                     # React 19 + Bootstrap 5
tests/                        # xUnit
```

Padrões: Clean Architecture, CQRS+MediatR, Repository Pattern, Unit of Work, DI.

## Entidades do Domínio
- `Contato` — lead/prospect/cliente/parceiro, com workflow Ativo ↔ Inativo
- `Oportunidade` — vinculada ao Contato, workflow: Aberta → EmAndamento → Ganha/Perdida/Cancelada

## Comandos Principais
```bash
# Backend
cd src/AMR.CRM.API && dotnet run
# → http://localhost:5187/swagger

# Frontend
cd frontend && npm install && npm run dev
# → http://localhost:5176

# Testes
dotnet test

# Migrations
dotnet ef migrations add <Nome> --project src/AMR.CRM.Infrastructure --startup-project src/AMR.CRM.API
dotnet ef database update --project src/AMR.CRM.Infrastructure --startup-project src/AMR.CRM.API
```

## Estado do Projeto — Sprint 7 (AMR-CRM)
- Scaffold inicial criado em 04/06/2026
- Entidades: Contato + Oportunidade
- CRUD Contatos + workflow Oportunidades
- Frontend: Dashboard, Contatos, Oportunidades
- Seed de dados demo aplicado

## Próximas Features
- Atividades / Histórico de interações por Contato
- Funil de vendas visual (Kanban)
- Relatórios e pipeline por período
- Integração via RabbitMQ/MassTransit com AMR-Core (sync clientes)

## Troubleshooting
| Problema | Solução |
|---|---|
| Porta errada no backend | `launchSettings.json` → `applicationUrl: http://localhost:5187` |
| CORS bloqueando frontend | `appsettings.Development.json` → `AllowedOrigins: http://localhost:5176` |
| MediatR não resolve handlers | Verificar `RegisterServicesFromAssembly` no `Program.cs` |
| Vite proxy não funciona | `vite.config.ts` → `target: http://localhost:5187` |
