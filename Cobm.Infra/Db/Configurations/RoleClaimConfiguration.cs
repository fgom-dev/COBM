using Cobm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cobm.Infra.Db.Configurations;

internal sealed class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.ToTable("RoleClaim");
        builder.HasKey(x => new
        {
            x.RoleId,
            x.Type,
            x.Value,
        });
        builder.Property(x => x.Type)
            .HasMaxLength(50);
        builder.Property(x => x.Value)
            .HasMaxLength(50);
        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.Property(x => x.UpdatedAt)
            .IsRequired();

        builder.HasOne(x => x.Role)
            .WithMany(x => x.RoleClaims)
            .HasForeignKey(x => x.RoleId);
    }
}