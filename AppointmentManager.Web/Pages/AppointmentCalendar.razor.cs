using System.Globalization;
using AppointmentManager.Shared;
using AppointmentManager.Web.HttpClients;
using AppointmentManager.Web.Models;
using AppointmentManager.Web.Services;
using Core.Extensions.CollectionRelated;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;

namespace AppointmentManager.Web.Pages;

public partial class AppointmentCalendar
{
    private DateTime? _pickerMonth = DateTime.Now.StartOfMonth(CultureInfo.CurrentCulture);
    private DateTime _selectedDate = DateTime.Today;
    private ICollection<AppointmentDto> _appointments = Array.Empty<AppointmentDto>();
    private AppointmentOverview _appointmentOverview = new();

    [Inject] public ThemeStateProvider ThemeStateProvider { get; set; }
    [Inject] public ApplicationManagerApiClient ApiClient { get; set; }
    [Inject] public ISnackbar Snackbar { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await DoInitializeAsync();

        await base.OnInitializedAsync();
    }

    private async Task DoInitializeAsync()
    {
        var from = _selectedDate.Date.StartOfMonth(CultureInfo.CurrentCulture).Date;
        var to = _selectedDate.Date.EndOfMonth(CultureInfo.CurrentCulture).Date.AddDays(1).AddTicks(-1);

        var searchFilter = new AppointmentSearchFilter(from, to);

        var option = await ApiClient.GetAppointmentsAsync(searchFilter);

        _appointments = option.ContinueWith(
            appointments => appointments,
            errors =>
            {
                Snackbar.Add(errors.ToSeparatedString("; "), Severity.Warning);
                return Array.Empty<AppointmentDto>();
            },
            Array.Empty<AppointmentDto>).ToList();

        await _appointmentOverview.ReloadServerData(_appointments.Where(appointment =>
            appointment.From.Date == _selectedDate.Date));
    }

    private string SetDateStyle(DateTime date)
    {
        if (DateIsTodayOrSelected(date) || !_appointments.Any(a => a.From.Date.Equals(date.Date)))
            return string.Empty;

        return ThemeStateProvider.GetCustomMarkedDateClass();
    }
    
    private bool DateIsTodayOrSelected(DateTime date)
    {
        var today = DateTime.Today;
        return HasSameYearMonthDay(date, today) || HasSameYearMonthDay(date, _selectedDate);
    }
    
    private static bool HasSameYearMonthDay(DateTime self, DateTime compareToTime)
    {
        return self.Year.Equals(compareToTime.Year) &&
               self.Month.Equals(compareToTime.Month) &&
               self.Day.Equals(compareToTime.Day);
    }

    private async Task ChangeMonthAsync(DateTime? date)
    {
        _pickerMonth = date!.Value.StartOfMonth(CultureInfo.CurrentCulture);

        await ChangeDate(_pickerMonth);
        await DoInitializeAsync();
    }
    
    private async Task ChangeDate(DateTime? time)
    {
        _selectedDate = time!.Value;
        await _appointmentOverview.ReloadServerData(_appointments.Where(appointment =>
            appointment.From.Date == _selectedDate.Date));
    }
}