using AppointmentManager.Shared;
using AppointmentManager.Web.Models;

namespace AppointmentManager.Web.HttpClients;

public class ApplicationManagerApiClient : BaseHttpClient
{
    public ApplicationManagerApiClient(HttpClient httpClient) : base(httpClient)
    {
    }
    
    public async Task<Option<TokenDto>> LoginAsync(LoginDto login, CancellationToken ct = default)
    {
        return await PostAsJsonAsync<LoginDto, TokenDto>("Authentication/login", login, ct);
    }

    public async Task<Option<TokenDto>> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        var refresh = new RefreshDto(refreshToken);
        
        return await PostAsJsonAsync<RefreshDto, TokenDto>("Authentication/refresh", refresh, ct);
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