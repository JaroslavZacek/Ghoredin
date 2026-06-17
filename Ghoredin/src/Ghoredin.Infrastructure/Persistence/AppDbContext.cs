using Ghoredin.Infrastructure.Identity;
using Ghoredin.Domain.Characters;

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
        }
    }
}
