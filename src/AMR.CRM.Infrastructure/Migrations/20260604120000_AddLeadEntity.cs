using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMR.CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id            = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome          = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Email         = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefone      = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Empresa       = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Status        = table.Column<int>(type: "INTEGER", nullable: false),
                    Origem        = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notas         = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CriadoEm      = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm    = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "LeadId",
                table: "Oportunidades",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Probabilidade",
                table: "Oportunidades",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Oportunidades_LeadId",
                table: "Oportunidades",
                column: "LeadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Oportunidades_LeadId",
                table: "Oportunidades");

            migrationBuilder.DropColumn(
                name: "LeadId",
                table: "Oportunidades");

            migrationBuilder.DropColumn(
                name: "Probabilidade",
                table: "Oportunidades");

            migrationBuilder.DropTable(name: "Leads");
        }
    }
}
