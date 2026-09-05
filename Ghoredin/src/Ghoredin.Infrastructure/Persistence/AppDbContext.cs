using Ghoredin.Infrastructure.Identity;
using Ghoredin.Domain.Characters;
using Ghoredin.Domain.Campaigns;
using Ghoredin.Domain.Notes;
using Ghoredin.Domain.Chat;
using Ghoredin.Domain.Handouts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;


namespace Ghoredin.Infrastructure.Persistence
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Character> Characters => Set<Character>();
        public DbSet<Campaign> Campaigns => Set<Campaign>();
        public DbSet<CampaignMember> CampaignMembers => Set<CampaignMember>();
        public DbSet<CampaignNote> CampaignNotes => Set<CampaignNote>();
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
        public DbSet<Handout> Handouts => Set<Handout>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Character entity konfigurace
            builder.Entity<Character>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
                entity.Property(c => c.GameSystemId).IsRequired().HasMaxLength(100);
                entity.Property(c => c.OwnerUserId).IsRequired();

                entity.Property(c => c.SheetData)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null)
                            ?? new Dictionary<string, object>(),
                        new ValueComparer<Dictionary<string, object>>(
                            (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(b, (JsonSerializerOptions?)null),
                            v => v == null ? 0 : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
                            v => JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!
                        )
                    );
            });

            // Campaign entity konfigurace
            builder.Entity<Campaign>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
                entity.Property(c => c.GameSystemId).IsRequired().HasMaxLength(100);

                entity.Property(c => c.GameSystemSettings)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null)
                            ?? new Dictionary<string, object>(),
                        new ValueComparer<Dictionary<string, object>>(
                            (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(b, (JsonSerializerOptions?)null),
                            v => v == null ? 0 : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
                            v => JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!
                        )
                    );

                entity.HasMany(c => c.Members)
                      .WithOne()
                      .HasForeignKey(m => m.CampaignId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CampaignMember entity konfigurace
            builder.Entity<CampaignMember>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.UserId).IsRequired();
                entity.Property(m => m.CharacterId);
            });

            // CampaignNote entity konfigurace
            builder.Entity<CampaignNote>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.AuthorUserId).IsRequired();
                entity.Property(n => n.Title).IsRequired().HasMaxLength(100);
                entity.Property(n => n.Content);
                entity.Property(n => n.PlayerFacingContent);
                entity.Property(n => n.Visibility).HasConversion<string>();
            });

            // ChatMessage entity konfigurace
            builder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.AuthorUserId).IsRequired();
                entity.Property(m => m.Content).IsRequired();
            });

            // Handout entity konfigurace
            builder.Entity<Handout>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Title).IsRequired().HasMaxLength(200);
                entity.Property(h => h.Content).IsRequired();
                entity.Property(h => h.ShareMode).HasConversion<string>();
                entity.Property(h => h.ContentType).HasConversion<string>();
            });
        }
    }
}
