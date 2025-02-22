using System.ComponentModel.DataAnnotations;
using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

public class AdminController : ApplicationControllerBase
{
    private readonly AdminService _adminService;

    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public Task<ActionResult> GetByIdAsync([Required] Guid id, CancellationToken ct) =>
        GetResultAsync<AdminDto>(() => _adminService.GetByIdAsync(id, ct));

    [Authorize]
    [HttpPut("{id:guid}")]
    public Task<ActionResult> UpdateAsync([Required] Guid id, AdminDto dto, CancellationToken ct) =>
        GetResultAsync<AdminDto>(() => _adminService.UpdateAsync(id, dto, ct));
}