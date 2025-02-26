using System.Net;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class ApplicationControllerBase : ControllerBase
{
    protected async Task<ActionResult> GetResultAsync<T>(Func<Task<ApiResult>> func)
        where T : class
    {
        ApiResult apiResult;

        try
        {
            apiResult = await func();
        }
        catch (Exception e)
        {
            return Problem(statusCode: (int)HttpStatusCode.InternalServerError, detail: e.Message,
                title: "Unhandled Error");
        }
        
        if (apiResult.Success)
        {
            if (apiResult is ItemApiResult<T> itemResult)
                return itemResult.Added
                    ? Created(BuildCreateUri(itemResult.Id.Value), itemResult.Item)
                    : Ok(itemResult.Item);

            return Ok();
        }

        return GetResult(apiResult);
    }

    protected async Task<ActionResult> GetResultAsync(Func<Task<ApiResult>> func)
    {
        ApiResult apiResult;

        try
        {
            apiResult = await func();
        }
        catch (Exception e)
        {
            return Problem(statusCode: (int)HttpStatusCode.InternalServerError, detail: e.Message,
                title: "Unhandled Error");
        }
        
        if (apiResult.Success)
            return Ok();

        return GetResult(apiResult);
    }

    private ActionResult GetResult(ApiResult apiResult)
    {
        if (apiResult.Errors.Count > 0)
            return BadRequest(apiResult.Errors);

        if (apiResult is NotFoundApiResult)
            return NotFound();

        return NoContent();
    }

    private string BuildCreateUri(Guid id)
    {
        var uriBuilder = new UriBuilder(Request.GetDisplayUrl());
        uriBuilder.Path += $"/{id}";
        return uriBuilder.Uri.ToString();
    }
}