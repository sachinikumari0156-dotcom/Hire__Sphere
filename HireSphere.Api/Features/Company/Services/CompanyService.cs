using HireSphere.Api.Data;
using HireSphere.Api.Data.Entities;
using HireSphere.Api.Features.Company.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HireSphere.Api.Features.Company.Services;

public interface ICompanyService
{
    Task<CompanyResponse?> GetCompanyByIdAsync(Guid id);
    Task<List<CompanyListResponse>> GetAllCompaniesAsync();
    Task<CompanyResponse?> CreateCompanyAsync(CreateCompanyRequest request);
    Task<CompanyResponse?> UpdateCompanyAsync(Guid id, UpdateCompanyRequest request);
    Task<bool> DeleteCompanyAsync(Guid id);
}

public class CompanyService : ICompanyService
{
    private readonly HireSphereDbContext _dbContext;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(HireSphereDbContext dbContext, ILogger<CompanyService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<CompanyResponse?> GetCompanyByIdAsync(Guid id)
    {
        try
        {
            var company = await _dbContext.Companies
                .Include(c => c.Jobs)
                .Include(c => c.Recruiters)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                return null;

            return MapToCompanyResponse(company);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving company {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<CompanyListResponse>> GetAllCompaniesAsync()
    {
        try
        {
            var companies = await _dbContext.Companies
                .OrderBy(c => c.Name)
                .Select(c => new CompanyListResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Website = c.Website,
                    Location = c.Location,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return companies;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving companies: {ex.Message}");
            return new List<CompanyListResponse>();
        }
    }

    public async Task<CompanyResponse?> CreateCompanyAsync(CreateCompanyRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return null;

            var company = new Data.Entities.Company
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Website = request.Website,
                Location = request.Location,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Company {company.Name} created with ID {company.Id}");

            return MapToCompanyResponse(company);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating company: {ex.Message}");
            return null;
        }
    }

    public async Task<CompanyResponse?> UpdateCompanyAsync(Guid id, UpdateCompanyRequest request)
    {
        try
        {
            var company = await _dbContext.Companies
                .Include(c => c.Jobs)
                .Include(c => c.Recruiters)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                return null;

            company.Name = request.Name;
            company.Description = request.Description;
            company.Website = request.Website;
            company.Location = request.Location;

            _dbContext.Companies.Update(company);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Company {company.Name} updated");

            return MapToCompanyResponse(company);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating company {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> DeleteCompanyAsync(Guid id)
    {
        try
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
                return false;

            _dbContext.Companies.Remove(company);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Company {id} deleted");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting company {id}: {ex.Message}");
            return false;
        }
    }

    private static CompanyResponse MapToCompanyResponse(Data.Entities.Company company)
    {
        return new CompanyResponse
        {
            Id = company.Id,
            Name = company.Name,
            Description = company.Description,
            Website = company.Website,
            Location = company.Location,
            CreatedAt = company.CreatedAt,
            JobCount = company.Jobs.Count,
            RecruiterCount = company.Recruiters.Count
        };
    }
}
