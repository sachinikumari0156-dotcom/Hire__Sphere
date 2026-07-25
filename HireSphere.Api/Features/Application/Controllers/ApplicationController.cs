using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HireSphere.Api.Common;
using HireSphere.Api.Features.Application.Dtos;
using HireSphere.Api.Features.Application.Services;

namespace HireSphere.Api.Features.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<ApplicationController> _logger;

    public ApplicationController(IApplicationService applicationService, ILogger<ApplicationController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ApplicationResponse>>> GetApplication(Guid id)
    {
        var application = await _applicationService.GetApplicationByIdAsync(id);
        if (application == null)
        {
            return NotFound(ApiResponse<ApplicationResponse>.FailureResponse("Application not found"));
        }

        return Ok(ApiResponse<ApplicationResponse>.SuccessResponse(application, "Application retrieved successfully"));
    }

    [HttpGet("job/{jobId}")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<ApiResponse<List<ApplicationListResponse>>>> GetApplicationsByJob(Guid jobId)
    {
        var applications = await _applicationService.GetApplicationsByJobAsync(jobId);
        return Ok(ApiResponse<List<ApplicationListResponse>>.SuccessResponse(applications, "Job applications retrieved successfully"));
    }

    [HttpGet("candidate/{candidateProfileId}")]
    [Authorize(Roles = "Admin,Candidate")]
    public async Task<ActionResult<ApiResponse<List<ApplicationListResponse>>>> GetApplicationsByCandidate(Guid candidateProfileId)
    {
        var applications = await _applicationService.GetApplicationsByCandidateAsync(candidateProfileId);
        return Ok(ApiResponse<List<ApplicationListResponse>>.SuccessResponse(applications, "Candidate applications retrieved successfully"));
    }

    [HttpPost]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<ApiResponse<ApplicationResponse>>> CreateApplication([FromBody] CreateApplicationRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(ApiResponse<ApplicationResponse>.FailureResponse("Invalid input", errors));
        }

        var application = await _applicationService.CreateApplicationAsync(request);
        if (application == null)
        {
            return BadRequest(ApiResponse<ApplicationResponse>.FailureResponse("Failed to create application. You may have already applied for this job."));
        }

        _logger.LogInformation($"Application {application.Id} created");
        return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, ApiResponse<ApplicationResponse>.SuccessResponse(application, "Application created successfully"));
    }

    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<ApiResponse<ApplicationResponse>>> UpdateApplicationStatus(Guid id, [FromBody] UpdateApplicationStatusRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(ApiResponse<ApplicationResponse>.FailureResponse("Invalid input", errors));
        }

        var application = await _applicationService.UpdateApplicationStatusAsync(id, request);
        if (application == null)
        {
            return BadRequest(ApiResponse<ApplicationResponse>.FailureResponse("Application not found or invalid status"));
        }

        _logger.LogInformation($"Application {id} status updated");
        return Ok(ApiResponse<ApplicationResponse>.SuccessResponse(application, "Application status updated successfully"));
    }

    [HttpPost("{id}/withdraw")]
    [Authorize(Roles = "Candidate")]
    public async Task<ActionResult<ApiResponse<object>>> WithdrawApplication(Guid id)
    {
        var success = await _applicationService.WithdrawApplicationAsync(id);
        if (!success)
        {
            return NotFound(ApiResponse<object>.FailureResponse("Application not found"));
        }

        _logger.LogInformation($"Application {id} withdrawn");
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Application withdrawn successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteApplication(Guid id)
    {
        var success = await _applicationService.DeleteApplicationAsync(id);
        if (!success)
        {
            return NotFound(ApiResponse<object>.FailureResponse("Application not found"));
        }

        _logger.LogInformation($"Application {id} deleted");
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Application deleted successfully"));
    }
}
