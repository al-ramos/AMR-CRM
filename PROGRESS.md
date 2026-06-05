# PROGRESS — AMR-CRM Sprint 7 (CONCLUÍDO)

## Status: COMPLETO — commit 3ac08ec pushed para main

---

## Resumo da Sprint 7

### Cards entregues

| Card | Status | Commit |
|---|---|---|
| 🏗️ Scaffold AMR-CRM (.NET 10 + React 19) | ✅ | c51f265 |
| 👤 CRUD Leads + pipeline de status | ✅ | 17823a5 |
| 💼 CRUD de Oportunidades vinculadas a Leads | ✅ | aa61eae |
| 📊 Dashboard CRM — pipeline Kanban + métricas | ✅ | 9778a15 |
| 🔔 Atividades / histórico de interações por Lead | ✅ | 3ac08ec |
| 🔗 Integração RabbitMQ/MassTransit com AMR-Core | ✅ | 3ac08ec |
| 📈 Relatórios CRM (Funil, Forecast, Análise) | ✅ | 3ac08ec |
| 🚀 Deploy AWS — Docker + ECS + CI/CD | ✅ | 3ac08ec |

---

## Deploy AWS — checklist final

- Dockerfile API (mcr.microsoft.com/dotnet/aspnet:10.0) ✅
- frontend/Dockerfile (node:22-alpine + nginx:alpine) ✅
- frontend/nginx.conf (SPA routing + proxy /api/) ✅
- docker-compose.yml (API :5187 + Web :5176) ✅
- .github/workflows/ci.yml (build + testes .NET 10) ✅
- .github/workflows/deploy-aws.yml (ECR → ECS amr-system :8083) ✅
- infra/terraform/ecr.tf — amr-crm/api + amr-crm/web ✅
- infra/terraform/ecs.tf — task definitions + services CRM ✅
- infra/terraform/alb.tf — listener porta 8083 ✅
- infra/terraform/efs.tf — amr-crm-sqlite ✅
- infra/terraform/networking.tf — SG porta 8083 ✅
- infra/terraform/iam.tf — ecsTaskRole CRM via for_each ✅
- infra/terraform/variables.tf — crm_api_tag, crm_web_tag ✅
- infra/terraform/outputs.tf — crm_url + ECR URLs ✅
- infra/terraform/apply.ps1 — URL CRM adicionada no output ✅
- README.md — instruções terraform apply documentadas ✅

## Para provisionar a infraestrutura

```powershell
cd C:\GitHub\AMR\infra\terraform
.\apply.ps1
```

URL produção: http://amr-system-1908797477.sa-east-1.elb.amazonaws.com:8083

---

## Notion

Card Deploy AWS: https://app.notion.com/p/376d35f21de581f7801ceb7db80fb352
Status → ✅ Concluído | date:Entrega: 2026-06-05
(Notion MCP indisponível nesta sessão — atualizar manualmente se necessário)
