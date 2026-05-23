using NpgsqlTypes;

namespace Core.Enums;

public enum TransmissionType
{
    [PgName("MANUAL")]
    MANUAL,
    
    [PgName("AUTOMATIC")]
    AUTOMATIC
}