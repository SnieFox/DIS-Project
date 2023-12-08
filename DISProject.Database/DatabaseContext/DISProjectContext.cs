using DISProject.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace DISProject.Database.DatabaseContext;

public class DISProjectContext : DbContext
{
    public DISProjectContext(DbContextOptions<DISProjectContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<People> People { get; set; }
    public virtual DbSet<Purchase> Purchases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<People>(entity =>
        {
            entity.ToTable("People");
            
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name);
            entity.Property(e => e.Age);
            entity.Property(e => e.Quantity);
            entity.Property(e => e.Price);
        });
        
        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.ToTable("Purchase");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Quantity);
            entity.Property(e => e.Status);
        });
    }
}