using AppointmentManager.Web.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AppointmentManager.Web.Validation;

public class AppointmentValidator : BaseValidator<Appointment>
{
    public AppointmentValidator(IStringLocalizer<ValidationError> localizer) : base(localizer)
    {
        RuleFor(item => item.Name)
            .NotEmpty()
            .WithMessage("Name is required");
        
        RuleFor(itm => itm.Email)
            .NotEmpty()
            .WithMessage("E-mail is required")
            .EmailAddress()
            .WithMessage("Not a valid e-mail");
    }
}