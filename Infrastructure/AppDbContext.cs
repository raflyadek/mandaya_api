using System.Numerics;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {}
    
    public DbSet<Planet> Planets => Set<Planet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Planet>(entity =>
        {
            entity.Property(p => p.GravityMS2)
                  .HasColumnName("gravity_m_s2");
        });
    }
}