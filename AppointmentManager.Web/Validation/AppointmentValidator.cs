using AppointmentManager.Web.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AppointmentManager.Web.Validation;

public class AppointmentValidator : BaseValidator<AppointmentDto>
{
    public AppointmentValidator(IStringLocalizer<ValidationError> localizer) : base(localizer)
    {
        RuleFor(item => item.Name)
            .NotEmpty()
            .WithMessage(CreateLocalizedErrorMessage("NameRequired"));
        
        RuleFor(itm => itm.Email)
            .NotEmpty()
            .WithMessage(CreateLocalizedErrorMessage("EmailRequired"))
            .EmailAddress()
            .WithMessage(CreateLocalizedErrorMessage("EmailNotValid"));
    }
}