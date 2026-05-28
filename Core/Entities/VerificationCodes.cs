using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

[Table("verification_codes")]
public class VerificationCodes
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string VerificationCode { get; set; }
    public bool IsUsed { get; set; }
    public DateTime ExpiresAt { get; set; }
}