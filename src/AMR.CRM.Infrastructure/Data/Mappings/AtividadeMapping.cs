using AMR.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMR.CRM.Infrastructure.Data.Mappings;

public class AtividadeMapping : IEntityTypeConfiguration<Atividade>
{
    public void Configure(EntityTypeBuilder<Atividade> b)
    {
        b.HasKey(a => a.Id);
        b.Property(a => a.Titulo).HasMaxLength(200).IsRequired();
        b.Property(a => a.Observacao).HasMaxLength(2000);

        b.HasOne(a => a.Lead)
         .WithMany()
         .HasForeignKey(a => a.LeadId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired(false);

        b.HasOne(a => a.Oportunidade)
         .WithMany()
         .HasForeignKey(a => a.OportunidadeId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired(false);
    }
}
