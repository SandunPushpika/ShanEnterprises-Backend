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
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<VehicleImages> VehicleImages { get; set; }
    public DbSet<Payments> Payments { get; set; }
    
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
        modelBuilder.HasPostgresEnum<BookingStatus>("booking_status");
        modelBuilder.HasPostgresEnum<PaymentStatus>("payment_status");
        modelBuilder.HasPostgresEnum<PaymentMethod>("payment_method");
        
        modelBuilder.Entity<Booking>()
            .Property(b => b.BookingStatus)
            .HasColumnName("booking_status");
        
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Customer)
            .WithMany()
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Vehicle)
            .WithMany()
            .HasForeignKey(b => b.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}