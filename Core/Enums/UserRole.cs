using NpgsqlTypes;

namespace Core.Enums;

public enum UserRole
{
    [PgName("ADMIN")]
    ADMIN,
    
    [PgName("CUSTOMER")]
    CUSTOMER,
    
    [PgName("DRIVER")]
    DRIVER
}