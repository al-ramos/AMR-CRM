using AMR.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMR.CRM.Infrastructure.Data.Mappings;

public class ContatoMapping : IEntityTypeConfiguration<Contato>
{
    public void Configure(EntityTypeBuilder<Contato> b)
    {
        b.HasKey(c => c.Id);
        b.Property(c => c.Nome).HasMaxLength(200).IsRequired();
        b.Property(c => c.Email).HasMaxLength(200).IsRequired();
        b.Property(c => c.Telefone).HasMaxLength(30);
        b.Property(c => c.Empresa).HasMaxLength(200);
        b.Property(c => c.Notas).HasMaxLength(2000);

        b.HasMany(c => c.Oportunidades)
         .WithOne(o => o.Contato)
         .HasForeignKey(o => o.ContatoId)
         .IsRequired(false)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
