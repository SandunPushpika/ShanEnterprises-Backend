using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.DTOs.Request.Customer;

public class CustomerStatusUpdateRequest
{
    [Required]
    public UserStatus Status { get; set; }
}