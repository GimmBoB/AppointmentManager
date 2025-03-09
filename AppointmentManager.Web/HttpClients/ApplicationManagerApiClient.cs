using AppointmentManager.Shared;
using AppointmentManager.Web.Models;
using AppointmentManager.Web.Services;

namespace AppointmentManager.Web.HttpClients;

public class ApplicationManagerApiClient : BaseHttpClient
{
    public ApplicationManagerApiClient(HttpClient httpClient, CustomStateProvider stateProvider) : base(httpClient, stateProvider)
    {
    }

    public async Task<Option<IEnumerable<AppointmentTimeSlotDto>>> GetTimeSlotsAsync(TimeSlotSearchFilter searchFilter,
        CancellationToken ct = default)
    {
        return await PostAsJsonAsync<TimeSlotSearchFilter, IEnumerable<AppointmentTimeSlotDto>>("AppointmentTimeSlot/search",
            searchFilter, ct);
    }
    
    public async Task<Option<Dictionary<DateTime, List<AppointmentTimeSlotDto>>>> GetTimeSlotsByDateRangeAsync(FreeSlotSearchFilter searchFilter,
        CancellationToken ct = default)
    {
        return await PostAsJsonAsync<FreeSlotSearchFilter, Dictionary<DateTime, List<AppointmentTimeSlotDto>>>("AppointmentTimeSlot/searchByDateRange",
            searchFilter, ct);
    }

    public async Task<Option<Appointment>> AddAppointmentAsync(Appointment appointment, CancellationToken ct = default)
    {
        return await PostAsJsonAsync<Appointment, Appointment>("Appointment", appointment, ct);
    }

    public async Task<Option<IEnumerable<AppointmentCategory>>> GetCategoriesAsync(CancellationToken ct = default)
    {
        return await GetAsJsonAsync<IEnumerable<AppointmentCategory>>("AppointmentCategory/all", ct);
    }

    public async Task AddFileAsync(Guid appointmentId, MultipartFormDataContent file, CancellationToken ct = default)
    {
        await PostAsync($"AppointmentExtension/{appointmentId}", file, ct);
    }
}