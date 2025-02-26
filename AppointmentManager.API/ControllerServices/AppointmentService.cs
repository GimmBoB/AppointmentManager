using AppointmentManager.API.Email;
using AppointmentManager.API.Models;
using AppointmentManager.API.Repositories;
using AppointmentManager.API.Utilities;

namespace AppointmentManager.API.ControllerServices;

public class AppointmentService
{
    private readonly AppointmentRepository _repository;
    private readonly AppointmentTimeSlotRepository _timeSlotRepository;
    private readonly AppointmentCategoryRepository _categoryRepository;
    private readonly MailService _mailService;

    public AppointmentService(
        AppointmentRepository repository,
        AppointmentTimeSlotRepository timeSlotRepository,
        AppointmentCategoryRepository categoryRepository,
        MailService mailService)
    {
        _repository = repository;
        _timeSlotRepository = timeSlotRepository;
        _categoryRepository = categoryRepository;
        _mailService = mailService;
    }

    public async Task<ApiResult> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var appointment = await _repository.GetByIdAsync(id, ct);

        if (appointment is null)
            return NotFoundApiResult.NotFound();

        return ItemApiResult<AppointmentDto>.Succeeded(MapToDto(appointment));
    }

    public async Task<ApiResult> AddAsync(AppointmentDto dto, CancellationToken ct)
    {
        var appointments = await _repository.GetAsync(new AppointmentSearchFilter(dto.From, dto.To), ct);
        var timeslots = await _timeSlotRepository.GetAsync(new TimeSlotSearchFilter(dto.From.DayOfWeek), ct);
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, ct);

        var errors = Validate(dto, appointments, timeslots, category);

        if (errors.Count > 0)
            return ApiResult.Failure(errors);

        var result = await _repository.AddAsync(new Appointment
        {
            Name = dto.Name,
            Email = dto.Email,
            From = dto.From,
            To = dto.To,
            AppointmentCategory = category,
            ExtraWishes = dto.ExtraWishes,
            Status = AppointmentStatus.Requested
        }, ct);

        // TODO use templates and send to Admin and Mail Address from Appointment
        _ = _mailService.CreateAndSendMailFromTemplateAsync("", "Lord doof", "t.weigang@gmx.de", new object());
        
        return ItemApiResult<AppointmentDto>.Created(MapToDto(result));
    }

    public async Task<ApiResult> UpdateAsync(Guid id, AppointmentDto dto, CancellationToken ct)
    {
        var appointment = await _repository.GetByIdAsync(id, ct);

        if (appointment is null)
            return NotFoundApiResult.NotFound();
        
        var appointments = await _repository.GetAsync(new AppointmentSearchFilter(dto.From, dto.To), ct);
        var timeslots = await _timeSlotRepository.GetAsync(new TimeSlotSearchFilter(dto.From.DayOfWeek), ct);
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, ct);

        var errors = Validate(dto, appointments, timeslots, category);

        if (errors.Count > 0)
            return ApiResult.Failure(errors);

        appointment.Status = dto.Status;

        var result = await _repository.UpdateAsync(appointment, ct);

        // TODO use templates and send to Admin and Mail Address from Appointment
        _ = _mailService.CreateAndSendMailFromTemplateAsync("", "Lord doof", "t.weigang@gmx.de", new object());
        
        return ItemApiResult<AppointmentDto>.Succeeded(MapToDto(result));
    }

    public async Task<ApiResult> GetAsync(AppointmentSearchFilter searchFilter, CancellationToken ct)
    {
        var result = await _repository.GetAsync(searchFilter, ct);

        return ItemApiResult<ICollection<AppointmentDto>>.Succeeded(result.Select(MapToDto).ToList());
    }

    private static ICollection<string> Validate(AppointmentDto dto, List<Appointment> appointments,
        List<AppointmentTimeSlot> timeslots, AppointmentCategory? category)
    {
        var errors = new List<string>();

        if (category is null)
            errors.Add($"{nameof(AppointmentCategory)} has to be set");

        if (dto.From >= dto.To)
        {
            errors.Add($"{nameof(dto.From)} has to be smaller than {nameof(dto.To)}");
            return errors;
        }

        if (appointments.Where(appointment => appointment.Status != AppointmentStatus.Canceled)
            .Any(appointment => appointment.Id != dto.Id))
            errors.Add($"There is already an appointment {dto.From:U} - {dto.To:U}");

        var diff = dto.To - dto.From; 
        
        if (diff.TotalDays >= 1)
            errors.Add($"You can not make an {nameof(Appointment)} into the next day");

        if (errors.Count > 0)
            return errors;

        var from = TimeSpan.FromHours(dto.From.Hour);
        var to = TimeSpan.FromHours(dto.To.Hour);
        
        if (!timeslots.Any(slot => slot.From == from && slot.To == to))
            errors.Add($"No {nameof(AppointmentTimeSlot)} found for {dto.From:U} - {dto.To:U}");

        if (!RegExUtil.IsValidEmail(dto.Email))
            errors.Add($"{dto.Email} is not a valid email");
        
        return errors;
    }

    private static AppointmentDto MapToDto(Appointment appointment)
    {
        return new AppointmentDto(appointment.Id, appointment.AppointmentCategoryId, appointment.Name,
            appointment.Email, appointment.ExtraWishes, appointment.From, appointment.To, appointment.Status);
    }
}