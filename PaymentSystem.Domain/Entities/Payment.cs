namespace PaymentSystem.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Tariff { get; set; } = string.Empty;
    public string CheckFilePath { get; set; } = string.Empty;
    public string CheckFileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign key
    public int? UserId { get; set; }
    public User? User { get; set; }
}
