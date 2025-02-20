using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

public class AppointmentCategoryController : ApplicationControllerBase
{
    private readonly AppointmentCategoryService _appointmentCategoryService;

    public AppointmentCategoryController(AppointmentCategoryService appointmentCategoryService)
    {
        _appointmentCategoryService = appointmentCategoryService;
    }

    [HttpGet("all")]
    public Task<ActionResult> GetAllAsync() =>
        GetResultAsync<ICollection<AppointmentCategory>>(() => _appointmentCategoryService.GetAllAsync());

    [HttpPost]
    public Task<ActionResult> AddAsync(CategoryDto dto) =>
        GetResultAsync<CategoryDto>(() => _appointmentCategoryService.AddAsync(dto));

    [HttpPut("{id:guid}")]
    public Task<ActionResult> UpdateAsync(Guid id, CategoryDto dto) =>
        GetResultAsync<CategoryDto>(() => _appointmentCategoryService.UpdateAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public Task<ActionResult> DeleteAsync(Guid id) =>
        GetResultAsync<CategoryDto>(() => _appointmentCategoryService.DeleteAsync(id));
}