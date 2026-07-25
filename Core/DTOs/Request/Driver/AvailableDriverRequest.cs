namespace Core.DTOs.Request.Driver;

public class AvailableDriverRequest
{
    public DateTime PickupDatetime { get; set; }

    public DateTime ReturnDatetime { get; set; }
}
