using FluentValidation;
using PaymentSystem.Common.DTOs;

namespace PaymentSystem.BLL.Validators;

public class SignUpDtoValidator : AbstractValidator<SignUpDto>
{
    public SignUpDtoValidator()
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
    }
}
