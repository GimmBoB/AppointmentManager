using AppointmentManager.Shared;
using AppointmentManager.Web.Models;
using AppointmentManager.Web.Services;

namespace AppointmentManager.Web.HttpClients;

public class ApplicationManagerApiClient : BaseHttpClient
{
    public ApplicationManagerApiClient(HttpClient httpClient, CustomStateProvider stateProvider) : base(httpClient, stateProvider)
    {
    }

    public async Task<Option<IEnumerable<Timeslot>>> GetTimeSlotsAsync(TimeSlotSearchFilter searchFilter,
        CancellationToken ct = default)
    {
        return await PostAsJsonAsync<TimeSlotSearchFilter, IEnumerable<Timeslot>>("AppointmentTimeSlot/search",
            searchFilter, ct);
    }
    
    public async Task<Option<Dictionary<DateTime, List<Timeslot>>>> GetTimeSlotsByDateRangeAsync(FreeSlotSearchFilter searchFilter,
        CancellationToken ct = default)
    {
        return await PostAsJsonAsync<FreeSlotSearchFilter, Dictionary<DateTime, List<Timeslot>>>("AppointmentTimeSlot/searchByDateRange",
            searchFilter, ct);
    }

    public async Task<Option<AppointmentDto>> AddAppointmentAsync(AppointmentDto appointment, CancellationToken ct = default)
    {
        return await PostAsJsonAsync<AppointmentDto, AppointmentDto>("Appointment", appointment, ct);
    }
    
    public async Task<Option<IEnumerable<AppointmentDto>>> GetAppointmentsAsync(AppointmentSearchFilter searchFilter, CancellationToken ct = default)
    {
        return await PostAsJsonAsync<AppointmentSearchFilter, IEnumerable<AppointmentDto>>("Appointment/search", searchFilter, ct);
    }

    public async Task<Option<IEnumerable<AppointmentCategory>>> GetCategoriesAsync(CancellationToken ct = default)
    {
        return await GetAsJsonAsync<IEnumerable<AppointmentCategory>>("AppointmentCategory/all", ct);
    }
    
    public async Task<Option<AppointmentCategory>> GetCategoryAsync(Guid id, CancellationToken ct = default)
    {
        return await GetAsJsonAsync<AppointmentCategory>($"AppointmentCategory/{id}", ct);
    }

    public async Task<Option<AppointmentCategory>> AddCategoryAsync(AppointmentCategory category, CancellationToken ct = default)
    {
        return await PostAsJsonAsync<AppointmentCategory, AppointmentCategory>("AppointmentCategory", category, ct);
    }
    
    public async Task<Option<AppointmentCategory>> UpdateCategoryAsync(Guid id, AppointmentCategory category, CancellationToken ct = default)
    {
        return await PutAsJsonAsync<AppointmentCategory, AppointmentCategory>($"AppointmentCategory/{id}", category, ct);
    }
    
    public async Task<Option<AppointmentDto>> UpdateAppointmentAsync(Guid id, AppointmentDto appointment, CancellationToken ct = default)
    {
        return await PutAsJsonAsync<AppointmentDto, AppointmentDto>($"Appointment/{id}", appointment, ct);
    }

    public async Task DeleteCategoryAsync(AppointmentCategory category, CancellationToken ct = default)
    {
        await DeleteAsync($"AppointmentCategory/{category.Id}", ct);
    }
    
    public async Task<Option<Timeslot>> GetTimeslotAsync(Guid id, CancellationToken ct = default)
    {
        return await GetAsJsonAsync<Timeslot>($"AppointmentTimeSlot/{id}", ct);
    }
    
    public async Task<Option<Timeslot>> AddTimeslotAsync(Timeslot timeslot, CancellationToken ct = default)
    {
        return await PostAsJsonAsync<Timeslot, Timeslot>("AppointmentTimeSlot", timeslot, ct);
    }
    
    public async Task<Option<Timeslot>> UpdateTimeslotAsync(Guid id, Timeslot timeslot, CancellationToken ct = default)
    {
        return await PutAsJsonAsync<Timeslot, Timeslot>($"AppointmentTimeSlot/{id}", timeslot, ct);
    }

    public async Task DeleteTimeSlotAsync(Timeslot timeSlot, CancellationToken ct = default)
    {
        await DeleteAsync($"AppointmentTimeSlot/{timeSlot.Id}", ct);
    }

    public async Task AddFileAsync(Guid appointmentId, MultipartFormDataContent file, CancellationToken ct = default)
    {
        await PostAsync($"AppointmentExtension/{appointmentId}", file, ct);
    }
    
    public async Task<string> GetImageUrlAsync(Guid id)
    {
        var result = await GetByteArrayAndContentTypeAsync($"AppointmentExtension/{id}/image");

        return $"data:{result.Item2};base64,{Convert.ToBase64String(result.Item1)}";
    }
}