using Core.Entities;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleBrand> VehicleBrands { get; set; }
    public DbSet<VehicleType> VehicleTypes { get; set; }
    public DbSet<VerificationCodes> VerificationCodes { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresEnum<FuelType>();

        modelBuilder.HasPostgresEnum<TransmissionType>();

        modelBuilder.HasPostgresEnum<VehicleStatus>();
        modelBuilder.HasPostgresEnum<UserRole>("user_role");
        modelBuilder.HasPostgresEnum<UserStatus>("user_status");
    }
}