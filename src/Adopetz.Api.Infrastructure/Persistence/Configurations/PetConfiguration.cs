using Adopetz.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adopetz.Api.Infrastructure.Persistence.Configurations
{
    internal class PetConfiguration : IEntityTypeConfiguration<Pet>
    {
        public void Configure(EntityTypeBuilder<Pet> builder)
        {
            builder.ToTable("pets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .IsRequired();

            builder.Property(x => x.Breed)
                .HasColumnName("breed");

            builder.Property(x => x.RescueDate)
                .HasColumnName("rescue_date");

            builder.Property(x => x.Adopted)
                .HasColumnName("adopted")
                .IsRequired();

            builder.Property(x => x.AgeInYears)
                .HasColumnName("age_in_years");

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.HasMany(x => x.Vaccines)
                .WithOne(v => v.Pet)
                .HasForeignKey(v => v.PetId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Images)
                .WithOne(i => i.Pet)
                .HasForeignKey(i => i.PetId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
