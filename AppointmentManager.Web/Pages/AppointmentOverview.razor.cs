using AppointmentManager.Shared;
using AppointmentManager.Web.HttpClients;
using AppointmentManager.Web.Models;
using Core.Extensions.CollectionRelated;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppointmentManager.Web.Pages;

public partial class AppointmentOverview
{
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public ApplicationManagerApiClient ApiClient { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }

    private MudDataGrid<Appointment> _grid = new();
    private ICollection<Appointment> _appointments = Array.Empty<Appointment>();
    private bool _pageLoading;
    private bool _buttonsDisabled;
    private bool _showOverlay;
    private ICollection<AppointmentCategory> _categories;

    protected override async Task OnInitializedAsync()
    {
        
        var option = await ApiClient.GetCategoriesAsync();
        _categories = option.ContinueWith(
            categories => categories,
            errors =>
            {
                Snackbar.Add(errors.ToSeparatedString("; "), Severity.Warning);
                return new List<AppointmentCategory>();
            },
            Array.Empty<AppointmentCategory>).ToList();
        
        await base.OnInitializedAsync();
    }

    public async Task ReloadServerData(IEnumerable<Appointment> appointments)
    {
        _appointments = appointments.ToList();
        await _grid.ReloadServerData();
    }

    private async Task<GridData<Appointment>> ReloadServerDataAsync(GridState<Appointment> arg)
    {
        return new GridData<Appointment>
        {
            Items = _appointments.OrderBy(appointment => appointment.From),
            TotalItems = _appointments.Count
        };

    }
}