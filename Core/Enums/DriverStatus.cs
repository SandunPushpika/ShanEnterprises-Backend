using NpgsqlTypes;

namespace Core.Enums;

public enum DriverStatus
{
    [PgName("PENDING")]
    PENDING,

    [PgName("APPROVED")]
    APPROVED,

    [PgName("REJECTED")]
    REJECTED,

    [PgName("DEACTIVATED")]
    DEACTIVATED
}
