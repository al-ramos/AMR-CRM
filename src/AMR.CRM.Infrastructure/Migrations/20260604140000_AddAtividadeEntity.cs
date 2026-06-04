using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMR.CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAtividadeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Atividades",
                columns: table => new
                {
                    Id             = table.Column<Guid>(type: "TEXT", nullable: false),
                    OportunidadeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Tipo           = table.Column<int>(type: "INTEGER", nullable: false),
                    Descricao      = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    DataHora       = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Concluida      = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadoEm       = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AlteradoEm     = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atividades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atividades_Oportunidades_OportunidadeId",
                        column: x => x.OportunidadeId,
                        principalTable: "Oportunidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Atividades_OportunidadeId",
                table: "Atividades",
                column: "OportunidadeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Atividades");
        }
    }
}
