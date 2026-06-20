using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Core.Entities;

public class VehicleImages
{
    public long Id { get; set; }
    
    [Column("vehicle_id")]
    public int VehicleId { get; set; }
    
    [JsonIgnore]
    [ForeignKey(nameof(VehicleId))]
    public virtual Vehicle Vehicle { get; set; } = null!;
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}