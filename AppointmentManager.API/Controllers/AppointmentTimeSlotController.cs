using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

public class AppointmentTimeSlotController : ApplicationControllerBase
{
    private readonly AppointmentTimeSlotService _service;

    public AppointmentTimeSlotController(AppointmentTimeSlotService service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}")]
    public Task<ActionResult> GetByIdAsync(Guid id, CancellationToken ct) =>
        GetResultAsync<AppointmentTimeSlotDto>(() => _service.GetByIdAsync(id, ct));
    
    [HttpGet("all")]
    public Task<ActionResult> GetAllAsync(CancellationToken ct) =>
        GetResultAsync<ICollection<AppointmentTimeSlotDto>>(() => _service.GetAllAsync(ct));

    [HttpPost("search")]
    public Task<ActionResult> GetAsync(TimeSlotSearchFilter searchFilter, CancellationToken ct) =>
        GetResultAsync<ICollection<AppointmentTimeSlotDto>>(() => _service.GetAsync(searchFilter, ct));

    [HttpPost]
    public Task<ActionResult> AddAsync(AppointmentTimeSlotDto dto, CancellationToken ct) =>
        GetResultAsync<AppointmentTimeSlotDto>(() => _service.AddAsync(dto, ct));

    [HttpPut("{id:guid}")]
    public Task<ActionResult> UpdateAsync(Guid id, AppointmentTimeSlotDto dto, CancellationToken ct) =>
        GetResultAsync<AppointmentTimeSlotDto>(() => _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    public Task<ActionResult> DeleteAsync(Guid id, CancellationToken ct) =>
        GetResultAsync(() => _service.DeleteAsync(id,ct));
}