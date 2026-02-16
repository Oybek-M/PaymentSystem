namespace PaymentSystem.Common.DTOs;

public class PaymentResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Tariff { get; set; } = string.Empty;
    public string CheckFileName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
