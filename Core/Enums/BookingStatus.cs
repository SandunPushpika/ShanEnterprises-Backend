using NpgsqlTypes;

namespace Core.Enums;

public enum BookingStatus
{
    [PgName("PENDING")] 
    PENDING,

    [PgName("CONFIRMED")]
    CONFIRMED,
    
    [PgName("ONGOING")]
    ONGOING,
    
    [PgName("COMPLETED")]
    COMPLETED,
    
    [PgName("CANCELLED")]
    CANCELLED,
    
    [PgName("REJECTED")]
    REJECTED
}