using NpgsqlTypes;

namespace Core.Enums;

public enum MaintenanceStatus
{
    
    [PgName("COMPLETED")]
    COMPLETED,
    
    [PgName("UNDER_MAINTENANCE")]
    UNDER_MAINTENANCE,
}