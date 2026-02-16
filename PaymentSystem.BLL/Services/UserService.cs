using PaymentSystem.BLL.Validators;
using PaymentSystem.Common.DTOs;
using PaymentSystem.Common.Helpers;
using PaymentSystem.DAL.Repositories;
using PaymentSystem.Domain.Entities;

namespace PaymentSystem.BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> SignUpAsync(SignUpDto signUpDto)
    {
        // Normalize phone number (remove spaces)
        var normalizedPhone = signUpDto.PhoneNumber.Replace(" ", "");

        // Check if user already exists
        var existingUser = await _userRepository.GetByPhoneNumberAsync(normalizedPhone);
        if (existingUser != null)
        {
            throw new Exception("Bu telefon raqam bilan foydalanuvchi allaqachon ro'yxatdan o'tgan");
        }

        var user = new User
        {
            FullName = signUpDto.FullName.Trim(),
            PhoneNumber = normalizedPhone,
            Tariff = signUpDto.Tariff.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user);

        return MapToUserResponseDto(createdUser);
    }

    public async Task<PagedList<UserResponseDto>> GetAllUsersAsync(PaginationParams paginationParams)
    {
        var totalCount = await _userRepository.CountAsync();
        var users = await _userRepository.GetPagedAsync(paginationParams.PageNumber, paginationParams.PageSize);

        var userDtos = users.Select(MapToUserResponseDto).ToList();

        return new PagedList<UserResponseDto>(
            userDtos,
            totalCount,
            paginationParams.PageNumber,
            paginationParams.PageSize
        );
    }

    private static UserResponseDto MapToUserResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Tariff = user.Tariff,
            CreatedAt = user.CreatedAt
        };
    }
}
