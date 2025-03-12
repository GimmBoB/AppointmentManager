using AppointmentManager.Web.HttpClients;
using AppointmentManager.Web.Models;
using AppointmentManager.Web.Validation;
using Core.Extensions.CollectionRelated;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppointmentManager.Web.Pages;

public partial class AppointmentTimeslotCard
{
    private Timeslot _item = new()
    {
        Id = Guid.Empty,
        Day = DayOfWeek.Monday,
        From = TimeSpan.FromHours(8),
        To = TimeSpan.FromHours(16)
    };
    private MudForm _form = new();
    private bool _showOverlay;
    private string _selectedDay;
    private MudTimePicker _fromPicker = new();
    private MudTimePicker _toPicker = new();

    [Inject] private IBaseValidator<Timeslot> Validator { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private ApplicationManagerApiClient ApiClient { get; set; }
    
    [Parameter] public Guid? Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Id.HasValue)
        {
            var option = await ApiClient.GetTimeslotAsync(Id.Value);
            _item = option.ContinueWith(
                timeslot => timeslot,
                errors =>
                {
                    Snackbar.Add($"An error occured: {errors.ToSeparatedString("; ")}", Severity.Warning);
                    Navigation.TryNavigateToReturnUrl();
                    return new Timeslot();
                },
                () =>
                {
                    Snackbar.Add($"Could not load timeslot", Severity.Warning);
                    Navigation.TryNavigateToReturnUrl();
                    return new Timeslot();
                });
            
        }
        
        _selectedDay = _item.Day.ToString();
                
        await base.OnInitializedAsync();
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
                    ? await ApiClient.AddTimeslotAsync(_item) 
                    : await ApiClient.UpdateTimeslotAsync(Id.Value, _item);

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

    private async Task ChangeFromAsync(TimeSpan? span, Timeslot item)
    {
        item.From = span ?? TimeSpan.Zero;
        await _toPicker.Validate();
    }

    private async Task ChangeToAsync(TimeSpan? span, Timeslot item)
    {
        item.To = span ?? TimeSpan.Zero;
        await _fromPicker.Validate();
    }
}