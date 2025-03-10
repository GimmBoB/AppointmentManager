using AppointmentManager.Shared;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace AppointmentManager.Web.Validation;

public class AppointmentCategoryValidator : BaseValidator<AppointmentCategory>
{
    public AppointmentCategoryValidator(IStringLocalizer<ValidationError> localizer) : base(localizer)
    {
        RuleFor(itm => itm.Name)
            .NotEmpty()
            .WithMessage("Name is required");
    }
}