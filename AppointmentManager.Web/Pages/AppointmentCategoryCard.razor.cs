using AppointmentManager.Shared;
using AppointmentManager.Web.HttpClients;
using AppointmentManager.Web.Validation;
using Core.Extensions.CollectionRelated;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppointmentManager.Web.Pages;

public partial class AppointmentCategoryCard
{
    private AppointmentCategory _item = new();
    private MudForm _form = new();
    private bool _showOverlay;

    [Inject] private IBaseValidator<AppointmentCategory> Validator { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private ApplicationManagerApiClient ApiClient { get; set; }
    
    [Parameter] public Guid? Id { get; set; }

    protected override Task OnInitializedAsync()
    {
        if (Id.HasValue)
        {
            // TODO load from db
        }
        return base.OnInitializedAsync();
    }

    private async Task<IEnumerable<string>> ValidateAsync(object model, string propertyName)
    {
        var errors = await Validator.ValidateValue(model, propertyName);
        return errors;
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

                var option = !Id.HasValue 
                    ? await ApiClient.AddCategoryAsync(_item) 
                    : await ApiClient.UpdateCategoryAsync(Id.Value, _item);

                _item = option.ContinueWith(appointment =>
                    {
                        Snackbar.Add("Operation was successful", Severity.Success);
                        Navigation.TryNavigateToReturnUrl();
                        return appointment;
                    },
                    errors =>
                    {
                        Snackbar.Add(errors.ToSeparatedString("; "), Severity.Warning);
                        return _item;
                    },
                    () => _item);
            }
            finally
            {
                _showOverlay = false;
            }
        }
    }
}