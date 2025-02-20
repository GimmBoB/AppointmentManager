using AppointmentManager.API.Models;
using AppointmentManager.API.Repositories;

namespace AppointmentManager.API.ControllerServices;

public class AppointmentTimeSlotService
{
    private readonly AppointmentTimeSlotRepository _repository;

    public AppointmentTimeSlotService(AppointmentTimeSlotRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResult> AddAsync(AppointmentTimeSlotDto dto)
    {
        var timeSlots = await _repository.GetAsync(new TimeSlotSearchFilter(dto.Day));

        var errors = Validate(Guid.Empty, dto, timeSlots);

        if (errors.Count > 0)
            return ApiResult.Failure(errors);

        var timeslot = await _repository.AddAsync(new AppointmentTimeSlot
            { Day = dto.Day, From = dto.From, To = dto.To });

        return ItemApiResult<AppointmentTimeSlotDto>.Succeeded(dto with{Id = timeslot.Id});
    }

    public async Task<ApiResult> UpdateAsync(Guid id, AppointmentTimeSlotDto dto)
    {
        var timeSlot = await _repository.GetByIdAsync(id);

        if (timeSlot is null)
            return ApiResult.NotFound();

        var timeSlots = await _repository.GetAsync(new TimeSlotSearchFilter(dto.Day));
        var errors = Validate(id, dto, timeSlots);

        if (errors.Count > 0)
            return ApiResult.Failure(errors);

        timeSlot.Day = dto.Day;
        timeSlot.From = dto.From;
        timeSlot.To = dto.To;

        await _repository.UpdateAsync(timeSlot);

        return ItemApiResult<AppointmentTimeSlotDto>.Succeeded(dto with { Id = id });
    }

    public async Task<ApiResult> GetAllAsync()
    {
        var result = await _repository.GetAsync(new TimeSlotSearchFilter(Days: null));

        return ItemApiResult<ICollection<AppointmentTimeSlot>>.Succeeded(result);
    }

    public async Task<ApiResult> GetAsync(TimeSlotSearchFilter searchFilter)
    {
        var result = await _repository.GetAsync(searchFilter);

        return ItemApiResult<ICollection<AppointmentTimeSlot>>.Succeeded(result);
    }

    public async Task<ApiResult> DeleteAsync(Guid id)
    {
        var timeSlot = await _repository.GetByIdAsync(id);

        if (timeSlot is null)
            return ApiResult.NotFound();
        
        await _repository.DeleteAsync(timeSlot);
        
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
            {
                errors.Add(
                    $"Overlapping time {timeSlot.Day.ToString()}:{timeSlot.From}-{timeSlot.To}");
            }
        }

        return errors;
    }
}