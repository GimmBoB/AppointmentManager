﻿using AppointmentManager.Shared;
using AppointmentManager.Web.HttpClients;
using AppointmentManager.Web.Shared;
using Core.Extensions.CollectionRelated;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppointmentManager.Web.Pages;

public partial class AppointmentCategoryList
{
    [Inject] private ApplicationManagerApiClient ApiClient { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    
    private MudDataGrid<AppointmentCategory> _grid = new();
    private bool _pageLoading;
    private bool _buttonsDisabled;
    private bool _showOverlay;

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

    private async Task DeleteAsync(AppointmentCategory category)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Do you want to delete '{category.Name}'?" },
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
                await ApiClient.DeleteCategoryAsync(category);
                
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