using NpgsqlTypes;

namespace Core.Enums;

public enum PaymentMethod
{
    [PgName("CARD")]
    CARD,
    
    [PgName("BANK_TRANSFER")]
    BANK_TRANSFER,
    
    [PgName("ONLINE")]
    ONLINE,
    
    [PgName("CASH")]
    CASH
}