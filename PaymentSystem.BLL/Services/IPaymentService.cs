using PaymentSystem.Common.DTOs;
using PaymentSystem.Common.Helpers;

namespace PaymentSystem.BLL.Services;

public interface IPaymentService
{
    Task<PaymentResponseDto> ProcessPaymentAsync(PayDto payDto);
    Task<PagedList<PaymentResponseDto>> GetAllPaymentsAsync(PaginationParams paginationParams);
}
