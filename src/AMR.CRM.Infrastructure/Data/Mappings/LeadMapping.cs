using AMR.CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AMR.CRM.Infrastructure.Data.Mappings;

public class LeadMapping : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> b)
    {
        b.HasKey(l => l.Id);
        b.Property(l => l.Nome).HasMaxLength(150).IsRequired();
        b.Property(l => l.Email).HasMaxLength(200).IsRequired();
        b.Property(l => l.Telefone).HasMaxLength(20);
        b.Property(l => l.Empresa).HasMaxLength(150);
        b.Property(l => l.Notas).HasMaxLength(2000);
        b.Property(l => l.ValorEstimado).HasColumnType("decimal(18,2)");
        b.Property(l => l.Status).HasConversion<int>();
        b.Property(l => l.Origem).HasConversion<int>();

        b.HasMany(l => l.Oportunidades)
         .WithOne(o => o.Lead)
         .HasForeignKey(o => o.LeadId)
         .IsRequired(false)
         .OnDelete(DeleteBehavior.SetNull);
    }
}
