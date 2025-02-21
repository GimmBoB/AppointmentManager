using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AppointmentController: ApplicationControllerBase
{
    private readonly AppointmentService _service;
    public AppointmentController(AppointmentService service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}")]
    public Task<ActionResult> GetByIdAsync(Guid id, CancellationToken ct) =>
        GetResultAsync<AppointmentDto>(() => _service.GetByIdAsync(id, ct));

    [HttpPost]
    public Task<ActionResult> AddAsync(AppointmentDto dto, CancellationToken ct) =>
        GetResultAsync<AppointmentDto>(() => _service.AddAsync(dto, ct));

    [HttpPost("search")]
    public Task<ActionResult> GetAsync(AppointmentSearchFilter searchFilter, CancellationToken ct) =>
        GetResultAsync<ICollection<AppointmentDto>>(() => _service.GetAsync(searchFilter, ct));

    [HttpPut("{id:guid}")]
    public Task<ActionResult> UpdateAsync(Guid id, AppointmentDto dto, CancellationToken ct) =>
        GetResultAsync<AppointmentDto>(() => _service.UpdateAsync(id, dto, ct));
}