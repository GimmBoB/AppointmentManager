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
    private Timeslot[] _timeSlotsOfDay = Array.Empty<Timeslot>();
    private Dictionary<DateTime, List<Timeslot>> _allTimeSlotsPerDate = new();
    private Timeslot? _selectedTimeSlot;
    private AppointmentCategory[] _categories = Array.Empty<AppointmentCategory>();
    private AppointmentCategory? _selectedCategory;

    [Inject] private IBaseValidator<Appointment> Validator { get; set; }
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

    private Task ChangeDateAsync(DateTime? date)
    {
        if (date is null)
        {
            _timeSlotDisabled = true;
            return Task.CompletedTask;
        }

        _selectedDate = date.Value;
        _timeSlotsOfDay = _allTimeSlotsPerDate.TryGetValue(date.Value.Date, out var slots)
            ? slots.ToArray()
            : Array.Empty<Timeslot>();

        _selectedTimeSlot = _timeSlotsOfDay.Any() ? _timeSlotsOfDay.MinBy(dto => dto.From) : null;

        _timeSlotDisabled = !_timeSlotsOfDay.Any();
        
        StateHasChanged();
        
        return Task.CompletedTask;
    }

    private async Task FetchTimeslotsAsync()
    {
        var option = await ApiClient.GetTimeSlotsByDateRangeAsync(new FreeSlotSearchFilter
        {
            From = DateTime.Now.Date, To = DateTime.Now.Date.AddDays(31).AddTicks(-1)
        });

        _allTimeSlotsPerDate = option.ContinueWith(
            dictionary =>
            {
                var first = dictionary.OrderBy(d => d.Key).Where(d => d.Value.Any()).Select(d => new
                    {
                        d
                            .Key,
                        d.Value
                    })
                    .FirstOrDefault();
                
                _selectedTimeSlot = first?.Value.Any() ?? false ? first.Value.MinBy(dto => dto.From) : null;
                _selectedDate = first?.Key ?? DateTime.Now;
                _timeSlotsOfDay = first?.Value.ToArray() ?? Array.Empty<Timeslot>();
                return dictionary;
            },
            errors =>
            {
                Snackbar.Add(errors.ToSeparatedString("; "), Severity.Warning);
                return new Dictionary<DateTime, List<Timeslot>>();
            },
            () => new Dictionary<DateTime, List<Timeslot>>());

        _timeSlotDisabled = !_timeSlotsOfDay.Any();
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
        const long maxAllowedSize = 4096 * 4096;
        if (_files.Count >= 3)
        {
            Snackbar.Add("You can only upload 3 files", Severity.Info);
            return;
        }

        var file = args.File;
        
        if (_files.Any(f => f.Name == file.Name))
        {
            Snackbar.Add("You already uploaded a file with the same name", Severity.Info);
            return;
        }

        if (file.Size > maxAllowedSize)
        {
            Snackbar.Add("File is to big. Maximum allowed size is 4096 * 4096 bytes", Severity.Info);
            return;
        }
        
        await using var fileStream = file.OpenReadStream(maxAllowedSize: maxAllowedSize);
        using var memoryStream = new MemoryStream();

        await fileStream.CopyToAsync(memoryStream);
        var byteArrayContent = new ByteArrayContent(memoryStream.ToArray());
        byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
        _files.Add(new File(byteArrayContent, file.Name));
        _inputFileCount++;
    }

    private void DeleteImage(int i)
    {
        _files.RemoveAt(i);
        _inputFileCount--;
    }
}