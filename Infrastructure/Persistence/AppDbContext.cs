
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // RefreshToken indexes
        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasIndex(rt => rt.Token).IsUnique();
            entity.HasIndex(rt => rt.UserId);
            entity.HasIndex(rt => rt.IsRevoked);
        });

       
        builder.Entity<AppRole>().HasData(
            new AppRole
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = SystemRoles.Admin,
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "1" // Required by IdentityRole<Guid>
            },
            new AppRole
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = SystemRoles.Manager,
                NormalizedName = "MANAGER",
                ConcurrencyStamp = "2"
            },
            new AppRole
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = SystemRoles.User,
                NormalizedName = "USER",
                ConcurrencyStamp = "3"
            }
        );
    }
}