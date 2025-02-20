using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

public class AppointmentTimeSlotController : ApplicationControllerBase
{
    private readonly AppointmentTimeSlotService _timeSlotService;

    public AppointmentTimeSlotController(AppointmentTimeSlotService timeSlotService)
    {
        _timeSlotService = timeSlotService;
    }

    [HttpGet("all")]
    public Task<ActionResult> GetAllAsync() =>
        GetResultAsync<ICollection<AppointmentTimeSlot>>(() => _timeSlotService.GetAllAsync());

    [HttpPost("search")]
    public Task<ActionResult> GetAsync(TimeSlotSearchFilter searchFilter) =>
        GetResultAsync<ICollection<AppointmentTimeSlot>>(() => _timeSlotService.GetAsync(searchFilter));

    [HttpPost]
    public Task<ActionResult> AddAsync(AppointmentTimeSlotDto dto) =>
        GetResultAsync<AppointmentTimeSlotDto>(() => _timeSlotService.AddAsync(dto));

    [HttpPut("{id:guid}")]
    public Task<ActionResult> UpdateAsync(Guid id, AppointmentTimeSlotDto dto) =>
        GetResultAsync<AppointmentTimeSlotDto>(() => _timeSlotService.UpdateAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public Task<ActionResult> DeleteAsync(Guid id) =>
        GetResultAsync<AppointmentTimeSlotDto>(() => _timeSlotService.DeleteAsync(id));
}