using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

public abstract class ApplicationControllerBase : ControllerBase
{
    protected ActionResult GetResult<T>(Result result)
        where T : class
    {
        if (result.Success)
        {
            if (result is ItemResult<T> itemResult)
            {
                return Ok(itemResult.Item);
            }

            return Ok();
        }

        if (result.Errors.Count > 0)
            return BadRequest(result.Errors);
        
        return NotFound();
    }
}