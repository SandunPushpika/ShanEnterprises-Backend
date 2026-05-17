using NpgsqlTypes;

namespace Core.Enums;

public enum UserStatus
{
    [PgName("ACTIVE")]
    ACTIVE,
    
    [PgName("INACTIVE")]
    INACTIVE,
    
    [PgName("CUSTOMER")]
    SUSPENDED,
}