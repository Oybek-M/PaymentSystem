using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.BLL.Services;
using PaymentSystem.Common.DTOs;
using PaymentSystem.Common.Helpers;
using PaymentSystem.Common.Responses;

namespace PaymentSystem.API.Controllers;

/// <summary>
/// Foydalanuvchilar bilan ishlash uchun endpoint'lar
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IValidator<SignUpDto> _signUpValidator;

    public UsersController(IUserService userService, IValidator<SignUpDto> signUpValidator)
    {
        _userService = userService;
        _signUpValidator = signUpValidator;
    }

    /// <summary>
    /// Yangi foydalanuvchini ro'yxatdan o'tkazish
    /// </summary>
    /// <remarks>
    /// Yangi foydalanuvchi yaratish uchun ushbu endpoint'dan foydalaning.
    /// 
    /// **Validatsiya qoidalari:**
    /// - **Ism:** 2-200 ta belgi, bo'sh bo'lmasligi kerak
    /// - **Telefon raqam:** +998 XX XXX XX XX formatida bo'lishi shart
    /// - **Tarif:** Bo'sh bo'lmasligi kerak, maksimum 100 ta belgi
    /// 
    /// **Telefon format misollari:**
    /// - ✅ +998 90 123 45 67
    /// - ✅ +998 91 234 56 78
    /// - ✅ +998901234567 (bo'sh joysiz ham bo'ladi)
    /// - ❌ 998901234567 (+ belgisi yo'q)
    /// - ❌ +998 9012345678 (format xato)
    /// 
    /// **Xato holatlari:**
    /// - Telefon raqam allaqachon ro'yxatdan o'tgan bo'lsa - 400 Bad Request
    /// - Validatsiya xatosi bo'lsa - 400 Bad Request
    /// 
    /// **Muvaffaqiyatli natija:**
    /// - Status: 200 OK
    /// - Yaratilgan foydalanuvchi ma'lumotlari qaytadi
    /// 
    /// **Sample Request:**
    /// 
    ///     POST /api/users/signup
    ///     {
    ///         "fullName": "Oybek Nuriddinov",
    ///         "phoneNumber": "+998 90 123 45 67",
    ///         "tariff": "Premium"
    ///     }
    /// 
    /// </remarks>
    /// <param name="signUpDto">Foydalanuvchi ma'lumotlari</param>
    /// <returns>Yaratilgan foydalanuvchi ma'lumotlari</returns>
    /// <response code="200">Foydalanuvchi muvaffaqiyatli ro'yxatdan o'tdi</response>
    /// <response code="400">Validatsiya xatosi yoki telefon raqam allaqachon ro'yxatdan o'tgan</response>
    /// <response code="500">Server xatosi</response>
    [HttpPost("signup")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> SignUp([FromBody] SignUpDto signUpDto)
    {
        try
        {
            // Validate
            var validationResult = await _signUpValidator.ValidateAsync(signUpDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<UserResponseDto>.ErrorResponse(
                    "Validatsiya xatosi", errors));
            }

            var user = await _userService.SignUpAsync(signUpDto);
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(
                user, "Foydalanuvchi muvaffaqiyatli ro'yxatdan o'tdi"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Barcha ro'yxatdan o'tgan foydalanuvchilar ro'yxatini olish
    /// </summary>
    /// <remarks>
    /// Tizimda ro'yxatdan o'tgan barcha foydalanuvchilarni pagination bilan olish.
    /// 
    /// **Pagination parametrlari:**
    /// - **pageNumber:** Sahifa raqami (minimum: 1, default: 1)
    /// - **pageSize:** Sahifadagi elementlar soni (minimum: 1, maximum: 100, default: 10)
    /// 
    /// **Response ma'lumotlari:**
    /// - **items:** Joriy sahifadagi foydalanuvchilar ro'yxati
    /// - **totalCount:** Jami foydalanuvchilar soni
    /// - **totalPages:** Jami sahifalar soni
    /// - **hasPrevious:** Oldingi sahifa mavjudmi?
    /// - **hasNext:** Keyingi sahifa mavjudmi?
    /// 
    /// **Misollar:**
    /// - Birinchi sahifa (10 ta element): `?pageNumber=1&amp;pageSize=10`
    /// - Ikkinchi sahifa (20 ta element): `?pageNumber=2&amp;pageSize=20`
    /// - Uchinchi sahifa (50 ta element): `?pageNumber=3&amp;pageSize=50`
    /// 
    /// **Limitlar:**
    /// - Maksimum pageSize: 100
    /// - Agar pageSize 100 dan katta bo'lsa, avtomatik 100 ga o'rnatiladi
    /// 
    /// **Sample Request:**
    /// 
    ///     GET /api/users?pageNumber=1&amp;pageSize=20
    /// 
    /// </remarks>
    /// <param name="paginationParams">Pagination parametrlari (pageNumber va pageSize)</param>
    /// <returns>Foydalanuvchilar ro'yxati pagination ma'lumotlari bilan</returns>
    /// <response code="200">Foydalanuvchilar ro'yxati muvaffaqiyatli qaytarildi</response>
    /// <response code="400">Xato yuz berdi</response>
    /// <response code="500">Server xatosi</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedList<UserResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedList<UserResponseDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<PagedList<UserResponseDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedList<UserResponseDto>>>> GetAllUsers(
        [FromQuery] PaginationParams paginationParams)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync(paginationParams);
            return Ok(ApiResponse<PagedList<UserResponseDto>>.SuccessResponse(
                users, "Foydalanuvchilar ro'yxati"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PagedList<UserResponseDto>>.ErrorResponse(ex.Message));
        }
    }
}
