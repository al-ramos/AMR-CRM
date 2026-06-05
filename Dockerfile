# ── Stage 1: Build .NET ───────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/AMR.CRM.API/AMR.CRM.API.csproj
RUN dotnet publish src/AMR.CRM.API/AMR.CRM.API.csproj -c Release -o /app/publish

# ── Stage 2: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# SQLite data dir
RUN mkdir -p /data
ENV ConnectionStrings__AmrCrm="Data Source=/data/amr-crm.db"
ENV ASPNETCORE_URLS="http://+:8080"
ENV ASPNETCORE_ENVIRONMENT="Production"

EXPOSE 8080
ENTRYPOINT ["dotnet", "AMR.CRM.API.dll"]
