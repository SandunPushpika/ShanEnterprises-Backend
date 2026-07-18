using NpgsqlTypes;

namespace Core.Enums;

public enum PaymentStatus
{
    [PgName("PENDING")]
    PENDING,
    
    [PgName("COMPLETED")]
    COMPLETED,
    
    [PgName("FAILED")]
    FAILED,
    
    [PgName("REFUNDED")]
    REFUNDED
}