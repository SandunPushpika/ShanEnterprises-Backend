using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Entities;

[Table("users")]
public class User
{
    [Key]
    public long Id { get; set; }
    
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    [Required, MaxLength(255)]
    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;

    [Required]
    public UserRole Role { get; set; } = UserRole.CUSTOMER;

    [Required]
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;

    public string? ProfileImageUrl { get; set; }
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(50)]
    public string? NicPassportNumber { get; set; }

    public bool EmailVerified { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}