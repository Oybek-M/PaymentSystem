using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PaymentSystem.BLL.Services;
using PaymentSystem.Common.DTOs;
using PaymentSystem.Common.Helpers;
using PaymentSystem.Common.Responses;

namespace PaymentSystem.API.Controllers;

/// <summary>
/// To'lovlar bilan ishlash uchun endpoint'lar
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IValidator<PayDto> _payValidator;

    public PaymentsController(IPaymentService paymentService, IValidator<PayDto> payValidator)
    {
        _paymentService = paymentService;
        _payValidator = payValidator;
    }

    /// <summary>
    /// To'lov qayd etish (Check fayli bilan)
    /// </summary>
    /// <remarks>
    /// Yangi to'lovni check fayli bilan birga qayd etish.
    /// 
    /// **MUHIM:** Bu endpoint `multipart/form-data` formatida ishlaydi!
    /// 
    /// **Form field'lari:**
    /// - **fullName** (string): To'lov qiluvchining to'liq ismi
    /// - **phoneNumber** (string): Telefon raqam (+998 XX XXX XX XX)
    /// - **tariff** (string): Tarif nomi
    /// - **checkFile** (file): Check fayli (PDF yoki rasm)
    /// 
    /// **Validatsiya qoidalari:**
    /// 
    /// *Ism:*
    /// - ✅ 2-200 ta belgi
    /// - ❌ Bo'sh bo'lmasligi kerak
    /// 
    /// *Telefon raqam:*
    /// - ✅ Format: +998 XX XXX XX XX
    /// - ✅ Misol: +998 90 123 45 67
    /// - ✅ Bo'sh joysiz ham bo'ladi: +998901234567
    /// - ❌ Boshqa formatlar qabul qilinmaydi
    /// 
    /// *Tarif:*
    /// - ✅ Maksimum 100 ta belgi
    /// - ❌ Bo'sh bo'lmasligi kerak
    /// 
    /// *Check fayli:*
    /// - ✅ **Qabul qilinadigan formatlar:** PDF, PNG, JPG, JPEG
    /// - ✅ **Maksimum hajm:** 5MB (5,242,880 bytes)
    /// - ❌ Boshqa formatlar rad etiladi
    /// - ❌ Bo'sh fayl rad etiladi
    /// 
    /// **File saqlash:**
    /// - Fayllar `wwwroot/uploads/` papkasiga saqlanadi
    /// - Har bir fayl unique GUID nomi bilan saqlanadi
    /// - Misol: `abc123-def456-ghi789.pdf`
    /// 
    /// **User bog'lanishi:**
    /// - Agar telefon raqam allaqachon ro'yxatdan o'tgan bo'lsa, to'lov shu user ga bog'lanadi
    /// - Aks holda, to'lov user'siz saqlanadi
    /// 
    /// **Swagger UI'da test qilish:**
    /// 1. "Try it out" tugmasini bosing
    /// 2. Barcha field'larni to'ldiring
    /// 3. "Choose File" orqali PDF yoki rasm tanlang
    /// 4. "Execute" tugmasini bosing
    /// 
    /// **cURL misoli:**
    /// 
    ///     curl -X POST "https://localhost:5001/api/payments/pay" \
    ///       -H "Content-Type: multipart/form-data" \
    ///       -F "fullName=Oybek Nuriddinov" \
    ///       -F "phoneNumber=+998 90 123 45 67" \
    ///       -F "tariff=Premium" \
    ///       -F "checkFile=@/path/to/receipt.pdf"
    /// 
    /// **Xato xabarlari:**
    /// - "Check fayli kiritilishi shart"
    /// - "Check fayli bo'sh bo'lmasligi kerak"
    /// - "Check fayli hajmi 5MB dan oshmasligi kerak"
    /// - "Faqat PDF, PNG, JPG, JPEG formatdagi fayllar qabul qilinadi"
    /// - "Telefon raqam formati noto'g'ri. Format: +998 XX XXX XX XX"
    /// 
    /// </remarks>
    /// <param name="payDto">To'lov ma'lumotlari va check fayli (multipart/form-data)</param>
    /// <returns>Qayd etilgan to'lov ma'lumotlari</returns>
    /// <response code="200">To'lov muvaffaqiyatli qayd etildi</response>
    /// <response code="400">Validatsiya xatosi yoki fayl xatosi</response>
    /// <response code="500">Server xatosi</response>
    [HttpPost("pay")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PaymentResponseDto>>> Pay([FromForm] PayDto payDto)
    {
        try
        {
            // Validate
            var validationResult = await _payValidator.ValidateAsync(payDto);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<PaymentResponseDto>.ErrorResponse(
                    "Validatsiya xatosi", errors));
            }

            var payment = await _paymentService.ProcessPaymentAsync(payDto);
            return Ok(ApiResponse<PaymentResponseDto>.SuccessResponse(
                payment, "To'lov muvaffaqiyatli qayd etildi"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PaymentResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Barcha to'lovlar ro'yxatini olish
    /// </summary>
    /// <remarks>
    /// Tizimda qayd etilgan barcha to'lovlarni pagination bilan olish.
    /// 
    /// **Pagination parametrlari:**
    /// - **pageNumber:** Sahifa raqami (minimum: 1, default: 1)
    /// - **pageSize:** Sahifadagi elementlar soni (minimum: 1, maximum: 100, default: 10)
    /// 
    /// **Response ma'lumotlari:**
    /// - **items:** Joriy sahifadagi to'lovlar ro'yxati
    /// - **totalCount:** Jami to'lovlar soni
    /// - **totalPages:** Jami sahifalar soni
    /// - **hasPrevious:** Oldingi sahifa mavjudmi?
    /// - **hasNext:** Keyingi sahifa mavjudmi?
    /// 
    /// **Har bir to'lov quyidagilarni o'z ichiga oladi:**
    /// - **id:** To'lov ID raqami
    /// - **fullName:** To'lov qiluvchining ismi
    /// - **phoneNumber:** Telefon raqam
    /// - **tariff:** Tarif nomi
    /// - **checkFileName:** Yuklangan check faylining nomi
    /// - **createdAt:** To'lov yaratilgan vaqt (UTC)
    /// 
    /// **Check faylini yuklab olish:**
    /// - Check fayllari `https://yourdomain.com/uploads/{checkFileName}` manzilida mavjud
    /// - Development: `https://localhost:5001/uploads/{checkFileName}`
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
    /// **Saralash:**
    /// - To'lovlar eng yangidan eskisiga qarab tartiblanadi
    /// - createdAt maydoni bo'yicha DESC order
    /// 
    /// **Sample Request:**
    /// 
    ///     GET /api/payments?pageNumber=1&amp;pageSize=20
    /// 
    /// </remarks>
    /// <param name="paginationParams">Pagination parametrlari (pageNumber va pageSize)</param>
    /// <returns>To'lovlar ro'yxati pagination ma'lumotlari bilan</returns>
    /// <response code="200">To'lovlar ro'yxati muvaffaqiyatli qaytarildi</response>
    /// <response code="400">Xato yuz berdi</response>
    /// <response code="500">Server xatosi</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedList<PaymentResponseDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedList<PaymentResponseDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<PagedList<PaymentResponseDto>>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PagedList<PaymentResponseDto>>>> GetAllPayments(
        [FromQuery] PaginationParams paginationParams)
    {
        try
        {
            var payments = await _paymentService.GetAllPaymentsAsync(paginationParams);
            return Ok(ApiResponse<PagedList<PaymentResponseDto>>.SuccessResponse(
                payments, "To'lovlar ro'yxati"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<PagedList<PaymentResponseDto>>.ErrorResponse(ex.Message));
        }
    }
}
