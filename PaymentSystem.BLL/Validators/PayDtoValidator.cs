using FluentValidation;
using PaymentSystem.Common.DTOs;

namespace PaymentSystem.BLL.Validators;

public class PayDtoValidator : AbstractValidator<PayDto>
{
    public PayDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ism kiritilishi shart")
            .MinimumLength(2).WithMessage("Ism kamida 2 ta belgidan iborat bo'lishi kerak")
            .MaximumLength(200).WithMessage("Ism 200 ta belgidan oshmasligi kerak");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon raqam kiritilishi shart")
            .Matches(@"^\+998\s?\d{2}\s?\d{3}\s?\d{2}\s?\d{2}$")
            .WithMessage("Telefon raqam formati noto'g'ri. Format: +998 XX XXX XX XX");

        RuleFor(x => x.Tariff)
            .NotEmpty().WithMessage("Tarif kiritilishi shart")
            .MaximumLength(100).WithMessage("Tarif nomi 100 ta belgidan oshmasligi kerak");

        RuleFor(x => x.CheckFile)
            .NotNull().WithMessage("Check fayli kiritilishi shart")
            .Must(file => file != null && file.Length > 0)
            .WithMessage("Check fayli bo'sh bo'lmasligi kerak")
            .Must(file => file == null || file.Length <= 5 * 1024 * 1024)
            .WithMessage("Check fayli hajmi 5MB dan oshmasligi kerak")
            .Must(file => file == null || 
                  file.ContentType == "application/pdf" || 
                  file.ContentType == "image/png" || 
                  file.ContentType == "image/jpeg" || 
                  file.ContentType == "image/jpg")
            .WithMessage("Faqat PDF, PNG, JPG, JPEG formatdagi fayllar qabul qilinadi");
    }
}
