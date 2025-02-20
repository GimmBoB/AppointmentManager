using System.ComponentModel.DataAnnotations;
using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

public class AdminController : ApplicationControllerBase
{
    private readonly AdminService _adminService;

    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("{id:guid}")]
    public Task<ActionResult> GetByIdAsync([Required] Guid id) => GetResultAsync<AdminDto>(() => _adminService.GetByIdAsync(id));

    [HttpPut("{id:guid}")]
    public Task<ActionResult> UpdateAsync([Required] Guid id, AdminDto dto) => GetResultAsync<AdminDto>(() => _adminService.UpdateAsync(id, dto));
}