using AppointmentManager.API.ControllerServices;
using AppointmentManager.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController : ApplicationControllerBase
{
    private readonly AdminService _adminService;

    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetAsync(Guid id)
    {
        var result = await _adminService.GetByIdAsync(id);

        return GetResult<AdminDto>(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAsync(Guid id, AdminDto dto)
    {
        var result = await _adminService.UpdateAsync(id, dto);

        return GetResult<AdminDto>(result);
    }
}