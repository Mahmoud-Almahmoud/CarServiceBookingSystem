using Asp.Versioning;
using CarServiceBookingSystem.Application.Interfaces.IBackgrounJobs;
using CarServiceBookingSystem.Application.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/background-jobs")]
public sealed class BackgroundJobsController : ControllerBase
{
    private readonly IBackgroundJobService _bgJobService;

    public BackgroundJobsController(IBackgroundJobService bgJobService)
    {
        _bgJobService = bgJobService;
    }

    [HttpGet("statistics")]
    [Authorize(Policy = Permissions.BackgroundJobs.View)]
    public IActionResult GetStatistics()
    {
        var response = _bgJobService.GetStatistics();
        return Ok(response);
    }

    [HttpGet("failed")]
    [Authorize(Policy = Permissions.BackgroundJobs.View)]
    public IActionResult GetFailedJobs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var response = _bgJobService.GetFailedJobs(page, pageSize);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpPost("failed/{jobId}/retry")]
    [Authorize(Policy = Permissions.BackgroundJobs.View)]
    public IActionResult RetryFailedJob(string jobId)
    {
        var response = _bgJobService.RetryFailedJob(jobId);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpDelete("failed/{jobId}")]
    [Authorize(Policy = Permissions.BackgroundJobs.View)]
    public IActionResult DeleteFailedJob(string jobId)
    {
        var response = _bgJobService.DeleteFailedJob(jobId);

        if (!response.Success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("recurring")]
    [Authorize(Policy = Permissions.BackgroundJobs.View)]
    public IActionResult GetRecurringJobs()
    {
        var response = _bgJobService.GetRecurringJobs();
        return Ok(response);
    }

    [HttpPost("recurring/{recurringJobId}/trigger")]
    [Authorize(Policy = Permissions.BackgroundJobs.View)]
    public IActionResult TriggerRecurringJob(string recurringJobId)
    {
        var response = _bgJobService.TriggerRecurringJob(recurringJobId);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("recurring/{recurringJobId}")]
    [Authorize(Policy = Permissions.BackgroundJobs.View)]
    public IActionResult GetRecurringJob(string recurringJobId)
    {
        var response = _bgJobService.GetRecurringJob(recurringJobId);

        if (!response.Success)
            return NotFound(response);

        return Ok(response);
    }
}