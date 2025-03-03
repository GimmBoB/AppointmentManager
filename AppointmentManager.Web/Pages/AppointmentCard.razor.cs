using AppointmentManager.Web.Models;
using AppointmentManager.Web.Validation;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppointmentManager.Web.Pages;

public partial class AppointmentCard
{
    private readonly Appointment _item = new();
    private MudForm _form = new();
    private bool _showOverlay;

    [Inject] private IBaseValidator<Appointment> _validator { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }
    
    private async Task<IEnumerable<string>> ValidateAsync(object model, string propertyName)
    {
        var errors = await _validator.ValidateValue(model, propertyName);
        return errors;
    }
    
    private void Cancel()
    {
        Navigation.TryNavigateToReturnUrl();
    }

    private async Task SubmitAsync()
    {
        await _form.Validate();

        if (_form.IsValid)
        {
            try
            {
                _showOverlay = true;

                StateHasChanged();

                // todo api calls
                
                Navigation.TryNavigateToReturnUrl();
            }
            finally
            {
                _showOverlay = false;
            }
        }
    }
}