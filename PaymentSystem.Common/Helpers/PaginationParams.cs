namespace PaymentSystem.Common.Helpers;

/// <summary>
/// Pagination parametrlari
/// </summary>
public class PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Sahifa raqami (minimum: 1, default: 1)
    /// </summary>
    /// <example>1</example>
    public int PageNumber { get; set; } = 1;
    
    /// <summary>
    /// Sahifadagi elementlar soni (minimum: 1, maximum: 100, default: 10)
    /// </summary>
    /// <example>10</example>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
