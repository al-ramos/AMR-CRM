using AMR.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMR.CRM.Infrastructure.Data.Mappings;

public class OportunidadeMapping : IEntityTypeConfiguration<Oportunidade>
{
    public void Configure(EntityTypeBuilder<Oportunidade> b)
    {
        b.HasKey(o => o.Id);
        b.Property(o => o.Titulo).HasMaxLength(300).IsRequired();
        b.Property(o => o.Valor).HasColumnType("decimal(18,2)");
        b.Property(o => o.Descricao).HasMaxLength(2000);
    }
}
