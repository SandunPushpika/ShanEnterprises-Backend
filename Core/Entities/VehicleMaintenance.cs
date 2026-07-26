using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Entities;

[Table("vehicle_service")]
public class VehicleMaintenance
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int VehicleId { get; set; }
    [ForeignKey(nameof(VehicleId))]
    public virtual Vehicle? Vehicle { get; set; }
    [Required]
    public DateOnly MaintenanceStart { get; set; }
    [Required]
    public DateOnly MaintenanceEnd { get; set; }
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public decimal Cost { get; set; }
    public MaintenanceStatus Status { get; set; } = MaintenanceStatus.UNDER_MAINTENANCE;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}