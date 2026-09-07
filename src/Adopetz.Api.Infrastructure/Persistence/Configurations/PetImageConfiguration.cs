using Adopetz.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Adopetz.Api.Infrastructure.Persistence.Configurations
{
    internal class PetImageConfiguration : IEntityTypeConfiguration<PetImage>
    {
        public void Configure(EntityTypeBuilder<PetImage> builder)
        {
            builder.ToTable("pet_images");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(x => x.PetId)
                .HasColumnName("pet_id")
                .IsRequired();

            builder.Property(x => x.FilePath)
                .HasColumnName("file_path")
                .IsRequired();
        }
    }
}
