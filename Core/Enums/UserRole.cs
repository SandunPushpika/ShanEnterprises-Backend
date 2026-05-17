using NpgsqlTypes;

namespace Core.Enums;

public enum UserRole
{
    [PgName("ADMIN")]
    ADMIN,
    
    [PgName("ADMIN")]
    CUSTOMER,
    
    [PgName("ADMIN")]
    DRIVER
}