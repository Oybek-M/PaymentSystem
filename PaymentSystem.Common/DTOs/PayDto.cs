using Microsoft.AspNetCore.Http;

namespace PaymentSystem.Common.DTOs;

/// <summary>
/// To'lov qilish uchun ma'lumotlar (multipart/form-data)
/// </summary>
public class PayDto
{
    /// <summary>
    /// To'lov qiluvchining to'liq ismi
    /// </summary>
    /// <example>Oybek Nuriddinov</example>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>
    /// Telefon raqam (+998 XX XXX XX XX formatida)
    /// </summary>
    /// <example>+998 90 123 45 67</example>
    public string PhoneNumber { get; set; } = string.Empty;
    
    /// <summary>
    /// Tarif nomi
    /// </summary>
    /// <example>Premium</example>
    public string Tariff { get; set; } = string.Empty;
    
    /// <summary>
    /// Check fayli (PDF, PNG, JPG, JPEG - maksimum 5MB)
    /// </summary>
    public IFormFile? CheckFile { get; set; }
}
