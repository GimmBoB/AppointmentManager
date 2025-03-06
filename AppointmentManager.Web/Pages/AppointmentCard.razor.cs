using System.Net.Http.Headers;
using AppointmentManager.Shared;
using AppointmentManager.Web.HttpClients;
using AppointmentManager.Web.Models;
using AppointmentManager.Web.Validation;
using Core.Extensions.CollectionRelated;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using File = AppointmentManager.Web.Models.File;

namespace AppointmentManager.Web.Pages;

public partial class AppointmentCard
{
    private Appointment _item = new();
    private MudForm _form = new();
    private bool _showOverlay;
    private DateTime _selectedDate = DateTime.Today;
    private int _inputFileCount = 0;
    private List<File> _files = new();
    private bool _timeSlotDisabled = true;
    private AppointmentTimeSlotDto[] _timeSlots = Array.Empty<AppointmentTimeSlotDto>();
    private AppointmentTimeSlotDto? _selectedTimeSlot;
    private AppointmentCategory[] _categories = Array.Empty<AppointmentCategory>();
    private AppointmentCategory? _selectedCategory;

    [Inject] private IBaseValidator<Appointment> _validator { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private ApplicationManagerApiClient ApiClient { get; set; }
    [Inject] private ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _selectedDate = DateTime.Today;
        await FetchTimeslotsAsync();
        await FetchCategoriesAsync();

        await base.OnInitializedAsync();
    }

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

                var from = new DateTime(_selectedDate.Year, _selectedDate.Month, _selectedDate.Day,
                    _selectedTimeSlot!.From.Hours, _selectedTimeSlot!.From.Minutes, 0, DateTimeKind.Local);
                var to = new DateTime(_selectedDate.Year, _selectedDate.Month, _selectedDate.Day,
                    _selectedTimeSlot!.To.Hours, _selectedTimeSlot!.To.Minutes, 0, DateTimeKind.Local);
                _item.From = from;
                _item.To = to;
                _item.CategoryId = _selectedCategory?.Id ?? Guid.Empty;

                var option = await ApiClient.AddAppointmentAsync(_item);

                _item = await option.ContinueWithAsync(
                    async appointment =>
                    {
                        await UploadFilesToServerAsync(appointment);
                        Snackbar.Add("Appointment send successfully", Severity.Success);
                        Navigation.TryNavigateToReturnUrl();
                        return appointment;
                    },
                    errors =>
                    {
                        Snackbar.Add(errors.ToSeparatedString("; "), Severity.Warning);
                        return Task.FromResult(_item);
                    },
                    () => Task.FromResult(_item));
            }
            finally
            {
                _showOverlay = false;
            }
        }
    }

    private async Task ChangeDateAsync(DateTime? date)
    {
        if (date is null)
        {
            _timeSlotDisabled = true;
            return;
        }

        _selectedDate = date.Value;
        
        await FetchTimeslotsAsync();
    }

    private async Task FetchTimeslotsAsync()
    {
        var option = await ApiClient.GetTimeSlotsAsync(new TimeSlotSearchFilter(_selectedDate.DayOfWeek, new FreeSlotSearchFilter
        {
            // start of day
            From = _selectedDate.Date,
            // end of day
            To = _selectedDate.Date.AddDays(1).AddTicks(-1)
        }));

        _timeSlots = option.ContinueWith(
            timeSlots =>
            {
                var slots = timeSlots.ToArray();
                _selectedTimeSlot = slots.FirstOrDefault();
                return slots;
            },
            errors =>
            {
                Snackbar.Add(errors.ToSeparatedString("; "), Severity.Warning);
                return Array.Empty<AppointmentTimeSlotDto>();
            },
            Array.Empty<AppointmentTimeSlotDto>);
        
        _timeSlotDisabled = !_timeSlots.Any();
    }

    private async Task FetchCategoriesAsync()
    {
        var option = await ApiClient.GetCategoriesAsync();
        
        _categories = option.ContinueWith(
            categories => categories.ToArray(),
            errors =>
            {
                Snackbar.Add(errors.ToSeparatedString("; "), Severity.Warning);
                return Array.Empty<AppointmentCategory>();
            },
            Array.Empty<AppointmentCategory>);

        _selectedCategory = _categories.FirstOrDefault();
    }

    private async Task UploadFilesToServerAsync(Appointment appointment)
    {
        foreach (var file in _files)
        {
            using var multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(file.Content, "file", file.Name);
            await ApiClient.AddFileAsync(appointment.Id, multipartFormDataContent);
        }
    }

    private async Task UploadFileAsync(InputFileChangeEventArgs args)
    {
        if (_files.Count >= 3)
        {
            Snackbar.Add("You can only upload 3 files", Severity.Info);
            return;
        }

        if (_files.Any(file => file.Name == args.File.Name))
        {
            Snackbar.Add("You already uploaded a file with the same name", Severity.Info);
            return;
        }
        var file = args.File;
        await using var fileStream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();

        await fileStream.CopyToAsync(memoryStream);
        var byteArrayContent = new ByteArrayContent(memoryStream.ToArray());
        byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        _files.Add(new File(byteArrayContent, file.Name));
        _inputFileCount++;
    }

    private string GetFileText(string fileName)
    {
        var maxLength = fileName.Length > 25 ? 25 : fileName.Length;
        var result = fileName[..maxLength];
        if (fileName.Length > maxLength)
        {
            result += "...";
        }

        return result;
    }
}