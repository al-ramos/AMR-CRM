using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMR.CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEtapaToOportunidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Etapa",
                table: "Oportunidades",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Etapa",
                table: "Oportunidades");
        }
    }
}
