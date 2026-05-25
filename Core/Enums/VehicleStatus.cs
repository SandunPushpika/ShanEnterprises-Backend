using NpgsqlTypes;

namespace Core.Enums;

public enum VehicleStatus
{
    [PgName("AVAILABLE")]
    AVAILABLE,
    
    [PgName("BOOKED")]
    BOOKED,
    
    [PgName("MAINTENANCE")]
    MAINTENANCE,
    
    [PgName("UNAVAILABLE")]
    UNAVAILABLE
}