using AppointmentManager.API.Models;
using AppointmentManager.API.Repositories;
using AppointmentManager.Shared;
using RazorLight.Extensions;

namespace AppointmentManager.API.ControllerServices;

public class AppointmentTimeSlotService
{
    private readonly AppointmentTimeSlotRepository _repository;

    public AppointmentTimeSlotService(AppointmentTimeSlotRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var timeslot = await _repository.GetByIdAsync(id, ct);

        if (timeslot is null)
            return NotFoundApiResult.NotFound();

        return ItemApiResult<AppointmentTimeSlotDto>.Succeeded(MapToDto(timeslot));
    }

    public async Task<ApiResult> AddAsync(AppointmentTimeSlotDto dto, CancellationToken ct)
    {
        var timeSlots = await _repository.GetAsync(new TimeSlotSearchFilter(dto.Day), ct);

        var errors = Validate(Guid.Empty, dto, timeSlots);

        if (errors.Count > 0)
            return ApiResult.Failure(errors);

        var timeslot = await _repository.AddAsync(new AppointmentTimeSlot
            { Day = dto.Day, From = dto.From, To = dto.To }, ct);

        return ItemApiResult<AppointmentTimeSlotDto>.Created(MapToDto(timeslot));
    }

    public async Task<ApiResult> UpdateAsync(Guid id, AppointmentTimeSlotDto dto, CancellationToken ct)
    {
        var timeSlot = await _repository.GetByIdAsync(id, ct);

        if (timeSlot is null)
            return NotFoundApiResult.NotFound();

        var timeSlots = await _repository.GetAsync(new TimeSlotSearchFilter(dto.Day), ct);
        var errors = Validate(id, dto, timeSlots);

        if (errors.Count > 0)
            return ApiResult.Failure(errors);

        timeSlot.Day = dto.Day;
        timeSlot.From = dto.From;
        timeSlot.To = dto.To;

        var result = await _repository.UpdateAsync(timeSlot, ct);

        return ItemApiResult<AppointmentTimeSlotDto>.Succeeded(MapToDto(result));
    }

    public async Task<ApiResult> GetAllAsync(CancellationToken ct)
    {
        var result = await _repository.GetAsync(new TimeSlotSearchFilter(Day: null), ct);

        return ItemApiResult<ICollection<AppointmentTimeSlotDto>>.Succeeded(result.Select(MapToDto).ToList());
    }

    public async Task<ApiResult> GetTimeSlotsPerDateRangeAsync(FreeSlotSearchFilter searchFilter, CancellationToken ct)
    {
        IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for(var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }

        if (searchFilter.From > searchFilter.To)
            return ApiResult.Failure(new List<string>
                { $"{nameof(searchFilter.From)} cant be greater than {nameof(searchFilter.To)}" });

        var eachDay = EachDay(searchFilter.From, searchFilter.To);

        var tasks = eachDay.Select(day => GetByDateAsync(day, ct)).ToList();

        var result = await Task.WhenAll(tasks);

        return ItemApiResult<Dictionary<DateTime, List<AppointmentTimeSlot>>>.Succeeded(result.SelectMany(r => r)
            .ToDictionary(r => r.Key, r => r.Value));
    }

    public async Task<ApiResult> GetAsync(TimeSlotSearchFilter searchFilter, CancellationToken ct)
    {
        var result = await _repository.GetAsync(searchFilter, ct);

        return ItemApiResult<ICollection<AppointmentTimeSlotDto>>.Succeeded(result.Select(MapToDto).ToList());
    }

    public async Task<ApiResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        var timeSlot = await _repository.GetByIdAsync(id, ct);

        if (timeSlot is null)
            return NotFoundApiResult.NotFound();
        
        await _repository.DeleteAsync(timeSlot, ct);
        
        return ApiResult.Succeeded();
    }

    private static ICollection<string> Validate(Guid id, AppointmentTimeSlotDto dto, List<AppointmentTimeSlot> timeSlots)
    {
        var errors = new List<string>();
        if (dto.From >= dto.To)
        {
            errors.Add($"{nameof(dto.From)} has to be smaller than {nameof(dto.To)}");
            return errors;
        }

        foreach (var timeSlot in timeSlots.Where(slot => slot.Id != id))
        {
            if (dto.From < timeSlot.To && dto.To > timeSlot.From)
                errors.Add(
                    $"Overlapping time {timeSlot.Day.ToString()}:{timeSlot.From}-{timeSlot.To}");
        }

        return errors;
    }

    private static AppointmentTimeSlotDto MapToDto(AppointmentTimeSlot timeSlot)
    {
        return new AppointmentTimeSlotDto(timeSlot.Id, timeSlot.Day, timeSlot.From, timeSlot.To);
    }
    
    private async Task<Dictionary<DateTime, List<AppointmentTimeSlot>>> GetByDateAsync(DateTime dateTime, CancellationToken ct)
    {
        var searchFilter = new TimeSlotSearchFilter(dateTime.DayOfWeek,
            new FreeSlotSearchFilter { From = dateTime.Date, To = dateTime.Date.AddDays(1).AddTicks(-1) });
        var result = await _repository.GetAsync(searchFilter, ct);

        return new Dictionary<DateTime, List<AppointmentTimeSlot>> { { dateTime, result } };
    }
}