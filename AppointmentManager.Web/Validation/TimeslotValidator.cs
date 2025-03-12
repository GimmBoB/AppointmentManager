using AppointmentManager.Web.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AppointmentManager.Web.Validation;

public class TimeslotValidator : BaseValidator<Timeslot>
{
    public TimeslotValidator(IStringLocalizer<ValidationError> localizer) : base(localizer)
    {
        RuleFor(itm => itm.From)
            .NotEmpty()
            .WithMessage("From is required")
            .LessThan(timeslot => timeslot.To)
            .WithMessage("From can not be greater than To");
        
        RuleFor(itm => itm.To)
            .NotEmpty()
            .WithMessage("To is required")
            .GreaterThan(timeslot => timeslot.From)
            .WithMessage("To can not be less than From");
    }
}