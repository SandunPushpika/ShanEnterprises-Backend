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
    public DbSet<VehicleMaintenance> VehicleMaintenances { get; set; }
    public DbSet<Payments> Payments { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<DriverBookingCancellation> DriverBookingCancellations { get; set; }
    public DbSet<ContactRequest> ContactRequests { get; set; }
    
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
        modelBuilder.HasPostgresEnum<MaintenanceStatus>("maintenance_status");

        modelBuilder.HasPostgresEnum<PaymentStatus>("payment_status");
        modelBuilder.HasPostgresEnum<PaymentMethod>("payment_method");
        modelBuilder.HasPostgresEnum<DriverStatus>("driver_status");
        modelBuilder.HasPostgresEnum<AvailabilityStatus>("availability_status");
        
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

        modelBuilder.Entity<VehicleMaintenance>()
            .HasOne(vm => vm.Vehicle)
            .WithMany()
            .HasForeignKey(vm => vm.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Booking)
            .WithMany()
            .HasForeignKey(r => r.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);



        modelBuilder.Entity<Review>()
            .HasOne(r => r.Vehicle)
            .WithMany()
            .HasForeignKey(r => r.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .Property(r => r.VehicleRating)
            .HasColumnName("vehicle_rating");


        modelBuilder.Entity<Review>()
            .Property(r => r.DriverRating)
            .HasColumnName("driver_rating");


        modelBuilder.Entity<Review>()
            .Property(r => r.CreatedAt)
            .HasColumnName("created_at");

        modelBuilder.Entity<Driver>()
            .HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Driver>()
            .HasOne(d => d.ApprovedByUser)
            .WithMany()
            .HasForeignKey(d => d.ApprovedBy)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Driver>()
            .Property(d => d.DriverStatus)
            .HasColumnName("driver_status");

        modelBuilder.Entity<Driver>()
            .HasIndex(d => d.UserId)
            .IsUnique();

        modelBuilder.Entity<Driver>()
            .HasIndex(d => d.LicenseNumber)
            .IsUnique();

        modelBuilder.Entity<DriverBookingCancellation>()
            .HasOne(d => d.Booking)
            .WithMany(b => b.DriverCancellations)
            .HasForeignKey(d => d.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DriverBookingCancellation>()
            .HasOne(d => d.Driver)
            .WithMany()
            .HasForeignKey(d => d.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ContactRequest>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}