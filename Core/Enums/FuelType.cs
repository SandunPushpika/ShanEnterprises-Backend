using NpgsqlTypes;

namespace Core.Enums;

public enum FuelType
{
    [PgName("PETROL")]
    PETROL,
    
    [PgName("DIESEL")]
    DIESEL,
    
    [PgName("HYBRID")]
    HYBRID,
    
    [PgName("ELECTRIC")]
    ELECTRIC
}