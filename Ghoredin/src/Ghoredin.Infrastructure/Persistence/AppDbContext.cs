using Ghoredin.Infrastructure.Identity;
using Ghoredin.Domain.Characters;
using Ghoredin.Domain.Campaigns;

using Microsoft.EntityFrameworkCore;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

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
                            ?? new Dictionary<string, object>()
                    );
            });

            builder.Entity<Campaign>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
                entity.Property(c => c.GameSystemId).IsRequired().HasMaxLength(100);

                entity.Property(c => c.GameSystemSettings)
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null)
                            ?? new Dictionary<string, object>()
                    );

                entity.HasMany(c => c.Members)
                      .WithOne()
                      .HasForeignKey(m => m.CampaignId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<CampaignMember>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.UserId).IsRequired();
            });
        }
    }
}
