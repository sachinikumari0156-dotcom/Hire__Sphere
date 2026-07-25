using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HireSphere.Api.Common;
using HireSphere.Api.Features.Job.Dtos;
using HireSphere.Api.Features.Job.Services;

namespace HireSphere.Api.Features.Job.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly ILogger<JobController> _logger;

    public JobController(IJobService jobService, ILogger<JobController> logger)
    {
        _jobService = jobService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<JobResponse>>> GetJob(Guid id)
    {
        var job = await _jobService.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound(ApiResponse<JobResponse>.FailureResponse("Job not found"));
        }

        return Ok(ApiResponse<JobResponse>.SuccessResponse(job, "Job retrieved successfully"));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<JobListResponse>>>> GetJobs([FromQuery] Guid? companyId)
    {
        var jobs = await _jobService.GetAllJobsAsync(companyId);
        return Ok(ApiResponse<List<JobListResponse>>.SuccessResponse(jobs, "Jobs retrieved successfully"));
    }

    [HttpGet("active")]
    public async Task<ActionResult<ApiResponse<List<JobListResponse>>>> GetActiveJobs()
    {
        var jobs = await _jobService.GetActiveJobsAsync();
        return Ok(ApiResponse<List<JobListResponse>>.SuccessResponse(jobs, "Active jobs retrieved successfully"));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<ApiResponse<JobResponse>>> CreateJob([FromBody] CreateJobRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(ApiResponse<JobResponse>.FailureResponse("Invalid input", errors));
        }

        var job = await _jobService.CreateJobAsync(request);
        if (job == null)
        {
            return BadRequest(ApiResponse<JobResponse>.FailureResponse("Failed to create job"));
        }

        _logger.LogInformation($"Job '{job.Title}' created");
        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, ApiResponse<JobResponse>.SuccessResponse(job, "Job created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<ApiResponse<JobResponse>>> UpdateJob(Guid id, [FromBody] UpdateJobRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(ApiResponse<JobResponse>.FailureResponse("Invalid input", errors));
        }

        var job = await _jobService.UpdateJobAsync(id, request);
        if (job == null)
        {
            return NotFound(ApiResponse<JobResponse>.FailureResponse("Job not found"));
        }

        _logger.LogInformation($"Job {id} updated");
        return Ok(ApiResponse<JobResponse>.SuccessResponse(job, "Job updated successfully"));
    }

    [HttpPost("{id}/close")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<ApiResponse<object>>> CloseJob(Guid id)
    {
        var success = await _jobService.CloseJobAsync(id);
        if (!success)
        {
            return NotFound(ApiResponse<object>.FailureResponse("Job not found"));
        }

        _logger.LogInformation($"Job {id} closed");
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Job closed successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteJob(Guid id)
    {
        var success = await _jobService.DeleteJobAsync(id);
        if (!success)
        {
            return NotFound(ApiResponse<object>.FailureResponse("Job not found"));
        }

        _logger.LogInformation($"Job {id} deleted");
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Job deleted successfully"));
    }
}
