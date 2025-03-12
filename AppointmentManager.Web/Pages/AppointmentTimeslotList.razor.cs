using AppointmentManager.Shared;
using AppointmentManager.Web.HttpClients;
using AppointmentManager.Web.Models;
using AppointmentManager.Web.Shared;
using Core.Extensions.CollectionRelated;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppointmentManager.Web.Pages;

public partial class AppointmentTimeslotList
{
    [Inject] private ApplicationManagerApiClient ApiClient { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    
    private MudDataGrid<Timeslot> _grid = new();
    private bool _pageLoading;
    private bool _buttonsDisabled;
    private bool _showOverlay;

    private async Task<GridData<Timeslot>> ReloadServerDataAsync(GridState<Timeslot> arg)
    {
        _pageLoading = true;
        _buttonsDisabled = true;

        
        List<Timeslot> appointmentTimeSlots;
        try
        {
            var option = await ApiClient.GetTimeSlotsAsync(new TimeSlotSearchFilter(null));

            appointmentTimeSlots = option.ContinueWith(
                timeslots => timeslots.OrderBy(timeslot => timeslot.Day).ToList(),
                errors =>
                {
                    Snackbar.Add(errors.ToSeparatedString("; "), Severity.Warning);
                    return new List<Timeslot>();
                },
                () => new List<Timeslot>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _pageLoading = false;
            _buttonsDisabled = false;
        }

        return new GridData<Timeslot>
        {
            Items = appointmentTimeSlots,
            TotalItems = appointmentTimeSlots.Count
        };
    }

    private async Task DeleteAsync(Timeslot timeslot)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Do you want to delete '{timeslot.From} - {timeslot.To}'?" },
            { "SubmitButtonText", "Delete" },
            { "CancelButtonText", "Cancel" },
            { "Color", Color.Error }
        };
        
        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, CloseOnEscapeKey = true };
        
        var answer = await (await DialogService
            .ShowAsync<CustomDialog>("Delete", parameters, options)).Result;

        if (!answer.Canceled)
        {
            try
            {
                _showOverlay = true;
                _buttonsDisabled = true;
                StateHasChanged();
                await ApiClient.DeleteTimeSlotAsync(timeslot);
                
                await _grid.ReloadServerData();

                Snackbar.Add("Item deleted", Severity.Success);
            }
            finally
            {
                _showOverlay = false;
                _buttonsDisabled = false;
                StateHasChanged();
            }
        }
    }
}