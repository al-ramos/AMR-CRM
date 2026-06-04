using AMR.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMR.CRM.Infrastructure.Data.Mappings;

public class AtividadeMapping : IEntityTypeConfiguration<Atividade>
{
    public void Configure(EntityTypeBuilder<Atividade> b)
    {
        b.HasKey(a => a.Id);
        b.Property(a => a.Descricao).HasMaxLength(2000).IsRequired();
        b.Property(a => a.Tipo).HasConversion<int>();

        b.HasOne(a => a.Oportunidade)
         .WithMany(o => o.Atividades)
         .HasForeignKey(a => a.OportunidadeId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
