using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

public class AppointmentCategoryController : ApplicationControllerBase
{
    private readonly AppointmentCategoryService _service;

    public AppointmentCategoryController(AppointmentCategoryService service)
    {
        _service = service;
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public Task<ActionResult> GetByIdAsync(Guid id, CancellationToken ct) =>
        GetResultAsync<CategoryDto>(() => _service.GetByIdAsync(id, ct));
    
    [Authorize]
    [HttpGet("all")]
    public Task<ActionResult> GetAllAsync(CancellationToken ct) =>
        GetResultAsync<ICollection<CategoryDto>>(() => _service.GetAllAsync(ct));

    [Authorize]
    [HttpPost]
    public Task<ActionResult> AddAsync(CategoryDto dto, CancellationToken ct) =>
        GetResultAsync<CategoryDto>(() => _service.AddAsync(dto, ct));

    [Authorize]
    [HttpPut("{id:guid}")]
    public Task<ActionResult> UpdateAsync(Guid id, CategoryDto dto, CancellationToken ct) =>
        GetResultAsync<CategoryDto>(() => _service.UpdateAsync(id, dto, ct));

    [Authorize]
    [HttpDelete("{id:guid}")]
    public Task<ActionResult> DeleteAsync(Guid id, CancellationToken ct) =>
        GetResultAsync(() => _service.DeleteAsync(id, ct));
}