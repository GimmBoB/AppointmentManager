using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

public class AppointmentTimeSlotController : ApplicationControllerBase
{
    private readonly AppointmentTimeSlotService _service;

    public AppointmentTimeSlotController(AppointmentTimeSlotService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public Task<ActionResult> GetByIdAsync(Guid id, CancellationToken ct) =>
        GetResultAsync<AppointmentTimeSlotDto>(() => _service.GetByIdAsync(id, ct));
    
    [Authorize]
    [HttpGet("all")]
    public Task<ActionResult> GetAllAsync(CancellationToken ct) =>
        GetResultAsync<ICollection<AppointmentTimeSlotDto>>(() => _service.GetAllAsync(ct));

    [Authorize]
    [HttpPost("search")]
    public Task<ActionResult> GetAsync(TimeSlotSearchFilter searchFilter, CancellationToken ct) =>
        GetResultAsync<ICollection<AppointmentTimeSlotDto>>(() => _service.GetAsync(searchFilter, ct));

    [Authorize]
    [HttpPost]
    public Task<ActionResult> AddAsync(AppointmentTimeSlotDto dto, CancellationToken ct) =>
        GetResultAsync<AppointmentTimeSlotDto>(() => _service.AddAsync(dto, ct));

    [Authorize]
    [HttpPut("{id:guid}")]
    public Task<ActionResult> UpdateAsync(Guid id, AppointmentTimeSlotDto dto, CancellationToken ct) =>
        GetResultAsync<AppointmentTimeSlotDto>(() => _service.UpdateAsync(id, dto, ct));

    [Authorize]
    [HttpDelete("{id:guid}")]
    public Task<ActionResult> DeleteAsync(Guid id, CancellationToken ct) =>
        GetResultAsync(() => _service.DeleteAsync(id,ct));
}