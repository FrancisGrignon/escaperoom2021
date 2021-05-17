using Backend.API.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.API.Infrastructure.EntityConfiguration
{
    public class TokenTypeConfiguration : IEntityTypeConfiguration<Token>
    {
        public void Configure(EntityTypeBuilder<Token> builder)
        {
            builder.ToTable("Token");

            builder.Property(ci => ci.Id)
                .UseHiLo("token_hilo")
                .IsRequired();

            builder.Property(cb => cb.Key)
                .IsRequired()
                .HasMaxLength(22);

            builder.Property(cb => cb.ContactId)
                .IsRequired();

            builder
                .HasOne(ci => ci.Contact)
                .WithMany(ci => ci.Tokens)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
