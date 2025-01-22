using HogwartApi.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace HogwartApi.Persistance;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
        });
        
        modelBuilder.Entity<Food>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
        });
        
        modelBuilder.Entity<FoodDiary>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
        });
        
        modelBuilder.Entity<Parking>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
        });
        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Parking> Parkings => Set<Parking>();
    public DbSet<Food> Foods => Set<Food>();
    public DbSet<FoodDiary> FoodDiaries => Set<FoodDiary>();
}