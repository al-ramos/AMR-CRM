using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AMR.CRM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeContatoIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQLite does not support ALTER COLUMN; rebuild the table.
            migrationBuilder.Sql(@"
PRAGMA foreign_keys = OFF;

CREATE TABLE ""Oportunidades_new"" (
    ""Id""                 TEXT NOT NULL CONSTRAINT ""PK_Oportunidades"" PRIMARY KEY,
    ""ContatoId""          TEXT,
    ""LeadId""             TEXT,
    ""Titulo""             TEXT NOT NULL,
    ""Valor""              TEXT NOT NULL,
    ""Probabilidade""      TEXT NOT NULL DEFAULT '0',
    ""Status""             INTEGER NOT NULL,
    ""Descricao""          TEXT,
    ""PrevisaoFechamento"" TEXT,
    ""CriadoEm""           TEXT NOT NULL,
    ""AlteradoEm""         TEXT NOT NULL,
    CONSTRAINT ""FK_Oportunidades_Contatos_ContatoId"" FOREIGN KEY (""ContatoId"") REFERENCES ""Contatos"" (""Id"") ON DELETE RESTRICT,
    CONSTRAINT ""FK_Oportunidades_Leads_LeadId"" FOREIGN KEY (""LeadId"") REFERENCES ""Leads"" (""Id"") ON DELETE SET NULL
);

INSERT INTO ""Oportunidades_new"" (""Id"", ""ContatoId"", ""LeadId"", ""Titulo"", ""Valor"", ""Probabilidade"", ""Status"", ""Descricao"", ""PrevisaoFechamento"", ""CriadoEm"", ""AlteradoEm"")
SELECT ""Id"", ""ContatoId"", ""LeadId"", ""Titulo"", ""Valor"", ""Probabilidade"", ""Status"", ""Descricao"", ""PrevisaoFechamento"", ""CriadoEm"", ""AlteradoEm""
FROM ""Oportunidades"";

DROP TABLE ""Oportunidades"";
ALTER TABLE ""Oportunidades_new"" RENAME TO ""Oportunidades"";

CREATE INDEX ""IX_Oportunidades_ContatoId"" ON ""Oportunidades"" (""ContatoId"");
CREATE INDEX ""IX_Oportunidades_LeadId"" ON ""Oportunidades"" (""LeadId"");

PRAGMA foreign_keys = ON;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
PRAGMA foreign_keys = OFF;

CREATE TABLE ""Oportunidades_old"" (
    ""Id""                 TEXT NOT NULL CONSTRAINT ""PK_Oportunidades"" PRIMARY KEY,
    ""ContatoId""          TEXT NOT NULL,
    ""LeadId""             TEXT,
    ""Titulo""             TEXT NOT NULL,
    ""Valor""              TEXT NOT NULL,
    ""Probabilidade""      TEXT NOT NULL DEFAULT '0',
    ""Status""             INTEGER NOT NULL,
    ""Descricao""          TEXT,
    ""PrevisaoFechamento"" TEXT,
    ""CriadoEm""           TEXT NOT NULL,
    ""AlteradoEm""         TEXT NOT NULL,
    CONSTRAINT ""FK_Oportunidades_Contatos_ContatoId"" FOREIGN KEY (""ContatoId"") REFERENCES ""Contatos"" (""Id"") ON DELETE RESTRICT,
    CONSTRAINT ""FK_Oportunidades_Leads_LeadId"" FOREIGN KEY (""LeadId"") REFERENCES ""Leads"" (""Id"") ON DELETE SET NULL
);

INSERT INTO ""Oportunidades_old"" (""Id"", ""ContatoId"", ""LeadId"", ""Titulo"", ""Valor"", ""Probabilidade"", ""Status"", ""Descricao"", ""PrevisaoFechamento"", ""CriadoEm"", ""AlteradoEm"")
SELECT ""Id"", COALESCE(""ContatoId"", '00000000-0000-0000-0000-000000000000'), ""LeadId"", ""Titulo"", ""Valor"", ""Probabilidade"", ""Status"", ""Descricao"", ""PrevisaoFechamento"", ""CriadoEm"", ""AlteradoEm""
FROM ""Oportunidades"";

DROP TABLE ""Oportunidades"";
ALTER TABLE ""Oportunidades_old"" RENAME TO ""Oportunidades"";

CREATE INDEX ""IX_Oportunidades_ContatoId"" ON ""Oportunidades"" (""ContatoId"");
CREATE INDEX ""IX_Oportunidades_LeadId"" ON ""Oportunidades"" (""LeadId"");

PRAGMA foreign_keys = ON;
");
        }
    }
}
