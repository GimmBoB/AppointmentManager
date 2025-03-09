using AppointmentManager.Shared;
using AppointmentManager.Web.HttpClients;
using Core.Extensions.CollectionRelated;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppointmentManager.Web.Pages;

public partial class AppointmentCategoryList
{
    [Inject] private ApplicationManagerApiClient ApiClient { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    
    private MudDataGrid<AppointmentCategory> _table = new();
    private bool _pageLoading;
    private bool _buttonsDisabled;

    private async Task<GridData<AppointmentCategory>> ReloadServerDataAsync(GridState<AppointmentCategory> arg)
    {
        _pageLoading = true;
        _buttonsDisabled = true;

        
        List<AppointmentCategory> appointmentCategories;
        try
        {
            var option = await ApiClient.GetCategoriesAsync(CancellationToken.None);

            appointmentCategories = option.ContinueWith(
                categories => categories.ToList(),
                errors =>
                {
                    Snackbar.Add(errors.ToSeparatedString("; "), Severity.Warning);
                    return new List<AppointmentCategory>();
                },
                () => new List<AppointmentCategory>());
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

        return new GridData<AppointmentCategory>
        {
            Items = appointmentCategories,
            TotalItems = appointmentCategories.Count
        };
    }
}