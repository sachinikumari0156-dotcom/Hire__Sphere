using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HireSphere.Api.Common;
using HireSphere.Api.Features.Company.Dtos;
using HireSphere.Api.Features.Company.Services;

namespace HireSphere.Api.Features.Company.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompanyController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger)
    {
        _companyService = companyService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> GetCompany(Guid id)
    {
        var company = await _companyService.GetCompanyByIdAsync(id);
        if (company == null)
        {
            return NotFound(ApiResponse<CompanyResponse>.FailureResponse("Company not found"));
        }

        return Ok(ApiResponse<CompanyResponse>.SuccessResponse(company, "Company retrieved successfully"));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CompanyListResponse>>>> GetAllCompanies()
    {
        var companies = await _companyService.GetAllCompaniesAsync();
        return Ok(ApiResponse<List<CompanyListResponse>>.SuccessResponse(companies, "Companies retrieved successfully"));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> CreateCompany([FromBody] CreateCompanyRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(ApiResponse<CompanyResponse>.FailureResponse("Invalid input", errors));
        }

        var company = await _companyService.CreateCompanyAsync(request);
        if (company == null)
        {
            return BadRequest(ApiResponse<CompanyResponse>.FailureResponse("Failed to create company"));
        }

        _logger.LogInformation($"Company {company.Name} created");
        return CreatedAtAction(nameof(GetCompany), new { id = company.Id }, ApiResponse<CompanyResponse>.SuccessResponse(company, "Company created successfully"));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Recruiter")]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> UpdateCompany(Guid id, [FromBody] UpdateCompanyRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(ApiResponse<CompanyResponse>.FailureResponse("Invalid input", errors));
        }

        var company = await _companyService.UpdateCompanyAsync(id, request);
        if (company == null)
        {
            return NotFound(ApiResponse<CompanyResponse>.FailureResponse("Company not found"));
        }

        _logger.LogInformation($"Company {id} updated");
        return Ok(ApiResponse<CompanyResponse>.SuccessResponse(company, "Company updated successfully"));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCompany(Guid id)
    {
        var success = await _companyService.DeleteCompanyAsync(id);
        if (!success)
        {
            return NotFound(ApiResponse<object>.FailureResponse("Company not found"));
        }

        _logger.LogInformation($"Company {id} deleted");
        return Ok(ApiResponse<object>.SuccessResponse(new { }, "Company deleted successfully"));
    }
}
