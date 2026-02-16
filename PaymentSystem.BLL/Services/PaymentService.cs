using PaymentSystem.Common.DTOs;
using PaymentSystem.Common.Helpers;
using PaymentSystem.DAL.Repositories;
using PaymentSystem.Domain.Entities;

namespace PaymentSystem.BLL.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserRepository _userRepository;
    private readonly string _uploadPath;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IUserRepository userRepository,
        string uploadPath)
    {
        _paymentRepository = paymentRepository;
        _userRepository = userRepository;
        _uploadPath = uploadPath;

        // Create upload directory if not exists
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    public async Task<PaymentResponseDto> ProcessPaymentAsync(PayDto payDto)
    {
        if (payDto.CheckFile == null)
        {
            throw new Exception("Check fayli yuklanmagan");
        }

        // Normalize phone number
        var normalizedPhone = payDto.PhoneNumber.Replace(" ", "");

        // Find or create user
        var user = await _userRepository.GetByPhoneNumberAsync(normalizedPhone);
        
        // Save file
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(payDto.CheckFile.FileName)}";
        var filePath = Path.Combine(_uploadPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await payDto.CheckFile.CopyToAsync(stream);
        }

        var payment = new Payment
        {
            FullName = payDto.FullName.Trim(),
            PhoneNumber = normalizedPhone,
            Tariff = payDto.Tariff.Trim(),
            CheckFilePath = filePath,
            CheckFileName = fileName,
            UserId = user?.Id,
            CreatedAt = DateTime.UtcNow
        };

        var createdPayment = await _paymentRepository.AddAsync(payment);

        return MapToPaymentResponseDto(createdPayment);
    }

    public async Task<PagedList<PaymentResponseDto>> GetAllPaymentsAsync(PaginationParams paginationParams)
    {
        var totalCount = await _paymentRepository.CountAsync();
        var payments = await _paymentRepository.GetPagedAsync(
            paginationParams.PageNumber, 
            paginationParams.PageSize);

        var paymentDtos = payments.Select(MapToPaymentResponseDto).ToList();

        return new PagedList<PaymentResponseDto>(
            paymentDtos,
            totalCount,
            paginationParams.PageNumber,
            paginationParams.PageSize
        );
    }

    private static PaymentResponseDto MapToPaymentResponseDto(Payment payment)
    {
        return new PaymentResponseDto
        {
            Id = payment.Id,
            FullName = payment.FullName,
            PhoneNumber = payment.PhoneNumber,
            Tariff = payment.Tariff,
            CheckFileName = payment.CheckFileName,
            CreatedAt = payment.CreatedAt
        };
    }
}
