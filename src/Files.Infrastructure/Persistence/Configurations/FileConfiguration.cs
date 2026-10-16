using Files.Domain.Files;
using Files.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = Files.Domain.Files.File;

namespace Files.Infrastructure.Persistence.Configurations;

public sealed class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.ToTable("files");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new FileId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.DeletedAt);

        builder.Property(x => x.OwnerId);

        builder.OwnsOne(
            x => x.Name,
            name =>
            {
                name.Property(p => p.Value)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .IsRequired();
            });

        builder.OwnsOne(
            x => x.ContentType,
            contentType =>
            {
                contentType.Property(p => p.Value)
                    .HasColumnName("content_type")
                    .HasMaxLength(255)
                    .IsRequired();
            });

        builder.Property(x => x.Size)
            .HasConversion(
                size => size.Bytes,
                bytes => FileSize.Create(bytes).Value)
            .IsRequired();

        builder.OwnsOne(
            x => x.Hash,
            hash =>
            {
                hash.Property(p => p.Value)
                    .HasColumnName("hash")
                    .HasMaxLength(128);

                hash.Property(p => p.Algorithm)
                    .HasColumnName("hash_algorithm")
                    .HasConversion<string>()
                    .HasMaxLength(32);
            });

        builder.OwnsOne(
            x => x.StorageKey,
            storageKey =>
            {
                storageKey.Property(p => p.Value)
                    .HasColumnName("storage_key")
                    .HasMaxLength(1024);
            });
    }
}
