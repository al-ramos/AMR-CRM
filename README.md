# AMR-CRM

Módulo CRM do AMR SYSTEM — gestão de contatos, leads, prospects e oportunidades de negócio.

**Stack:** .NET 10 + React 19 + SQLite | **Deploy:** AWS ECS Fargate + ECR + ALB (porta 8083)

---

## Dev local

```bash
# Backend
cd src/AMR.CRM.API && dotnet run
# → http://localhost:5187/swagger

# Frontend
cd frontend && npm install && npm run dev
# → http://localhost:5176

# Docker Compose (ambos juntos)
docker compose up --build
# API  → http://localhost:5187
# Web  → http://localhost:5176

# Testes
dotnet test
```

---

## Deploy AWS

### Pré-requisitos

- [Terraform 1.5+](https://terraform.io/downloads)
- [AWS CLI](https://aws.amazon.com/cli/) configurado (`aws configure`)
- Docker em execução
- GitHub Secrets configurados: `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`

### 1. Provisionar infraestrutura (primeira vez)

A infraestrutura do CRM é gerenciada junto com os demais módulos AMR no Terraform unificado em `infra/terraform/`.

```powershell
cd C:\GitHub\AMR\infra\terraform

# Opção A — script assistido (recomendado)
.\apply.ps1

# Opção B — comandos manuais
terraform init
terraform plan -out=tfplan
terraform apply tfplan
```

O Terraform cria para o CRM:
| Recurso | Nome/Detalhe |
|---|---|
| ECR API | `amr-crm/api` |
| ECR Web | `amr-crm/web` |
| ECS Cluster | `amr-system` (compartilhado) |
| ECS Service API | `amr-crm-api` |
| ECS Service Web | `amr-crm-web` |
| EFS | `amr-crm-sqlite` (SQLite persistente) |
| ALB Listener | porta **8083** |

### 2. Configurar variáveis Terraform

```powershell
# Copiar o exemplo
copy infra\terraform\terraform.tfvars.example infra\terraform\terraform.tfvars

# Editar e preencher:
# financeiro_jwt_key = "..."
# crm_api_tag        = "latest"   # ou SHA do commit
# crm_web_tag        = "latest"
```

### 3. Obter URL após o apply

```bash
terraform output crm_url
# → http://<alb-dns>:8083
```

### 4. CI/CD automático

Cada `push` para `main` dispara:
1. **CI** (`.github/workflows/ci.yml`) — build + testes .NET
2. **Deploy** (`.github/workflows/deploy-aws.yml`) — build Docker → push ECR → `ecs update-service` → health check

O deploy só ocorre se o CI passar.

### 5. Verificar serviços no ECS

```bash
# Status dos services
aws ecs describe-services \
  --cluster amr-system \
  --services amr-crm-api amr-crm-web \
  --region sa-east-1 \
  --query 'services[*].{Service:serviceName,Status:status,Running:runningCount}'

# Logs em tempo real
aws logs tail /ecs/amr-crm/api --follow --region sa-east-1
```

### 6. Destruir infra (se necessário)

```powershell
cd C:\GitHub\AMR\infra\terraform
.\apply.ps1 -Destroy
```

---

## Arquitetura

```
ALB :8083
  ├── /api/*  → ECS amr-crm-api  (ASP.NET Core :8080)
  │             └── EFS /amr-crm/amr-crm.db
  └── /*      → ECS amr-crm-web  (nginx :80)
```

## Portas locais

| Serviço | URL |
|---|---|
| API Swagger | http://localhost:5187/swagger |
| Frontend | http://localhost:5176 |
| RabbitMQ UI | http://localhost:15672 |
