using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

public class AppointmentCategoryController : ApplicationControllerBase
{
    private readonly AppointmentCategoryService _service;

    public AppointmentCategoryController(AppointmentCategoryService service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}")]
    public Task<ActionResult> GetByIdAsync(Guid id, CancellationToken ct) =>
        GetResultAsync<CategoryDto>(() => _service.GetByIdAsync(id, ct));
    
    [HttpGet("all")]
    public Task<ActionResult> GetAllAsync(CancellationToken ct) =>
        GetResultAsync<ICollection<CategoryDto>>(() => _service.GetAllAsync(ct));

    [HttpPost]
    public Task<ActionResult> AddAsync(CategoryDto dto, CancellationToken ct) =>
        GetResultAsync<CategoryDto>(() => _service.AddAsync(dto, ct));

    [HttpPut("{id:guid}")]
    public Task<ActionResult> UpdateAsync(Guid id, CategoryDto dto, CancellationToken ct) =>
        GetResultAsync<CategoryDto>(() => _service.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:guid}")]
    public Task<ActionResult> DeleteAsync(Guid id, CancellationToken ct) =>
        GetResultAsync<CategoryDto>(() => _service.DeleteAsync(id, ct));
}