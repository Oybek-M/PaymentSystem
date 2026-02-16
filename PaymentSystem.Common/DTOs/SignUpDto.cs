namespace PaymentSystem.Common.DTOs;

/// <summary>
/// Ro'yxatdan o'tish uchun ma'lumotlar
/// </summary>
public class SignUpDto
{
    /// <summary>
    /// Foydalanuvchining to'liq ismi
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
}
