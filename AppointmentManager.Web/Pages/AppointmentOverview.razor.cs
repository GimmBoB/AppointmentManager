using System.Text.RegularExpressions;
using AppointmentManager.Shared;
using AppointmentManager.Web.HttpClients;
using AppointmentManager.Web.Models;
using AppointmentManager.Web.Shared;
using Core.Extensions.CollectionRelated;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AppointmentManager.Web.Pages;

public partial class AppointmentOverview
{
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public ApplicationManagerApiClient ApiClient { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }
    [Inject] public IDialogService DialogService { get; set; }

    private MudDataGrid<AppointmentDto> _grid = new();
    private ICollection<AppointmentDto> _appointments = Array.Empty<AppointmentDto>();
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

    public async Task ReloadServerData(IEnumerable<AppointmentDto> appointments)
    {
        _appointments = appointments.ToList();
        await _grid.ReloadServerData();
    }

    private Task<GridData<AppointmentDto>> ReloadServerDataAsync(GridState<AppointmentDto> arg)
    {
        return Task.FromResult(new GridData<AppointmentDto>
        {
            Items = _appointments.OrderBy(appointment => appointment.From),
            TotalItems = _appointments.Count
        });

    }

    private string GetChangeStateTitle(AppointmentDto appointment)
    {
        return appointment.Status == AppointmentStatus.Accepted ? "Reject" : "Accept";
    }

    private async Task ViewImagesAsync(AppointmentDto appointment)
    {
        var files = new List<string>();

        foreach (var extension in appointment.AppointmentExtensions)
        {
            var imageSource = await ApiClient.GetImageUrlAsync(extension.Id);
            files.Add(imageSource);
        }
        
        var parameters = new DialogParameters
        {
            { "Images",  files}
        };
        
        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, CloseOnEscapeKey = true };
        
        var _ = await (await DialogService
            .ShowAsync<ImageDialog>("Images", parameters, options)).Result;
    }

    private bool GetChangeStateDisabled(AppointmentDto appointment)
    {
        return appointment.Status == AppointmentStatus.Requested;
    }

    private async Task UpdateStateAsync(AppointmentDto appointment, AppointmentStatus status)
    {
        var parameters = new DialogParameters
        {
            { "ContentText", $"Do you want to update the state to '{status.ToString()}'?" },
            { "SubmitButtonText", "Submit" },
            { "CancelButtonText", "Cancel" },
            { "Color", Color.Error }
        };
        
        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, FullWidth = true, CloseOnEscapeKey = true };
        
        var answer = await (await DialogService
            .ShowAsync<CustomDialog>("Change State", parameters, options)).Result;

        if (answer.Canceled)
            return;
        
        var oldState = appointment.Status;
        appointment.Status = status;
        var option = await ApiClient.UpdateAppointmentAsync(appointment.Id, appointment);

        option.ContinueWith(
            a =>
            {
                appointment.Status = a.Status;
                return a;
            },
            errors =>
            {
                appointment.Status = oldState;
                Snackbar.Add(errors.ToSeparatedString("; "), Severity.Warning);
                return appointment;
            },
            () =>
            {
                appointment.Status = oldState;
                return appointment;
            });
    }

    private Color GetColor(AppointmentDto appointment)
    {
        return appointment.Status switch
        {
            AppointmentStatus.Accepted => Color.Success,
            AppointmentStatus.Canceled => Color.Error,
            _ => Color.Warning
        };
    }

    private string GetPhoneNumber(AppointmentDto contextItem)
    {
        if (string.IsNullOrWhiteSpace(contextItem.CountryCode))
            return contextItem.PhoneNumber ?? string.Empty;
        
        return $"{MyRegex().Match(contextItem.CountryCode).Groups[1].Value} {contextItem.PhoneNumber ?? string.Empty}";
    }

    [GeneratedRegex("\\(([^)]*)\\)")]
    private static partial Regex MyRegex();
}