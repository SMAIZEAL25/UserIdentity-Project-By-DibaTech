using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Index for fast lookup on RefreshToken
            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(rt => rt.Token).IsUnique();
                entity.HasIndex(rt => rt.UserId);
                entity.HasIndex(rt => rt.IsRevoked);
            });

            // Seed default roles
            builder.Entity<AppRole>().HasData(
                new AppRole { Id = Guid.NewGuid(), Name = SystemRoles.Admin, NormalizedName = SystemRoles.Admin.ToUpper() },
                new AppRole { Id = Guid.NewGuid(), Name = SystemRoles.Manager, NormalizedName = SystemRoles.Manager.ToUpper() },
                new AppRole { Id = Guid.NewGuid(), Name = SystemRoles.User, NormalizedName = SystemRoles.User.ToUpper() }
            );
        }
    }
}
