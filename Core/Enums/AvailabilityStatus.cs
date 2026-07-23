using NpgsqlTypes;

namespace Core.Enums;

public enum AvailabilityStatus
{
    [PgName("AVAILABLE")]
    AVAILABLE,

    [PgName("UNAVAILABLE")]
    UNAVAILABLE,

    [PgName("ON_TRIP")]
    ON_TRIP
}
