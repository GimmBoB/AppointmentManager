using AppointmentManager.Web.Models;
using Microsoft.Extensions.Localization;

namespace AppointmentManager.Web.Validation;

public class AppointmentValidator : BaseValidator<Appointment>
{
    public AppointmentValidator(IStringLocalizer<ValidationError> localizer) : base(localizer)
    {
    }
}