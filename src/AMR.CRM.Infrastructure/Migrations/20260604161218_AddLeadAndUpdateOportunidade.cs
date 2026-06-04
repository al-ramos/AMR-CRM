using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMR.CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadAndUpdateOportunidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ContatoId",
                table: "Oportunidades",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<Guid>(
                name: "LeadId",
                table: "Oportunidades",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Probabilidade",
                table: "Oportunidades",
                type: "INTEGER",
                nullable: false,
                defaultValue: 50);

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: true),
                    Empresa = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Origem = table.Column<int>(type: "INTEGER", nullable: false),
                    ValorEstimado = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notas = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Oportunidades_LeadId",
                table: "Oportunidades",
                column: "LeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Oportunidades_Leads_LeadId",
                table: "Oportunidades",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Oportunidades_Leads_LeadId",
                table: "Oportunidades");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropIndex(
                name: "IX_Oportunidades_LeadId",
                table: "Oportunidades");

            migrationBuilder.DropColumn(
                name: "LeadId",
                table: "Oportunidades");

            migrationBuilder.DropColumn(
                name: "Probabilidade",
                table: "Oportunidades");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContatoId",
                table: "Oportunidades",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
