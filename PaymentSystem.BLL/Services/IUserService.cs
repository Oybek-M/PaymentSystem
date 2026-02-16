using PaymentSystem.Common.DTOs;
using PaymentSystem.Common.Helpers;

namespace PaymentSystem.BLL.Services;

public interface IUserService
{
    Task<UserResponseDto> SignUpAsync(SignUpDto signUpDto);
    Task<PagedList<UserResponseDto>> GetAllUsersAsync(PaginationParams paginationParams);
}
