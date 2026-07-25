using HireSphere.Api.Data;
using HireSphere.Api.Data.Entities;
using HireSphere.Api.Features.Job.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HireSphere.Api.Features.Job.Services;

public interface IJobService
{
    Task<JobResponse?> GetJobByIdAsync(Guid id);
    Task<List<JobListResponse>> GetAllJobsAsync(Guid? companyId = null);
    Task<List<JobListResponse>> GetActiveJobsAsync();
    Task<JobResponse?> CreateJobAsync(CreateJobRequest request);
    Task<JobResponse?> UpdateJobAsync(Guid id, UpdateJobRequest request);
    Task<bool> CloseJobAsync(Guid id);
    Task<bool> DeleteJobAsync(Guid id);
}

public class JobService : IJobService
{
    private readonly HireSphereDbContext _dbContext;
    private readonly ILogger<JobService> _logger;

    public JobService(HireSphereDbContext dbContext, ILogger<JobService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<JobResponse?> GetJobByIdAsync(Guid id)
    {
        try
        {
            var job = await _dbContext.Jobs
                .Include(j => j.Company)
                .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
                .Include(j => j.Applications)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return null;

            return MapToJobResponse(job);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving job {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<JobListResponse>> GetAllJobsAsync(Guid? companyId = null)
    {
        try
        {
            var query = _dbContext.Jobs
                .Include(j => j.Company)
                .Include(j => j.Applications)
                .AsQueryable();

            if (companyId.HasValue)
                query = query.Where(j => j.CompanyId == companyId.Value);

            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Select(j => new JobListResponse
                {
                    Id = j.Id,
                    Title = j.Title,
                    Location = j.Location,
                    SalaryRange = j.SalaryRange,
                    CompanyId = j.CompanyId,
                    CompanyName = j.Company.Name,
                    IsClosed = j.IsClosed,
                    CreatedAt = j.CreatedAt,
                    ApplicationCount = j.Applications.Count
                })
                .ToListAsync();

            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving jobs: {ex.Message}");
            return new List<JobListResponse>();
        }
    }

    public async Task<List<JobListResponse>> GetActiveJobsAsync()
    {
        try
        {
            var jobs = await _dbContext.Jobs
                .Include(j => j.Company)
                .Include(j => j.Applications)
                .Where(j => !j.IsClosed)
                .OrderByDescending(j => j.CreatedAt)
                .Select(j => new JobListResponse
                {
                    Id = j.Id,
                    Title = j.Title,
                    Location = j.Location,
                    SalaryRange = j.SalaryRange,
                    CompanyId = j.CompanyId,
                    CompanyName = j.Company.Name,
                    IsClosed = j.IsClosed,
                    CreatedAt = j.CreatedAt,
                    ApplicationCount = j.Applications.Count
                })
                .ToListAsync();

            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving active jobs: {ex.Message}");
            return new List<JobListResponse>();
        }
    }

    public async Task<JobResponse?> CreateJobAsync(CreateJobRequest request)
    {
        try
        {
            // Verify company exists
            var company = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == request.CompanyId);
            if (company == null)
            {
                _logger.LogWarning($"Company {request.CompanyId} not found");
                return null;
            }

            var job = new Data.Entities.Job
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                SalaryRange = request.SalaryRange,
                IsClosed = false,
                CompanyId = request.CompanyId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Jobs.Add(job);
            await _dbContext.SaveChangesAsync();

            // Add required skills if provided
            if (request.RequiredSkillIds != null && request.RequiredSkillIds.Any())
            {
                foreach (var skillId in request.RequiredSkillIds)
                {
                    var skill = await _dbContext.Skills.FirstOrDefaultAsync(s => s.Id == skillId);
                    if (skill != null)
                    {
                        _dbContext.JobSkills.Add(new JobSkill { JobId = job.Id, SkillId = skillId });
                    }
                }
                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation($"Job '{job.Title}' created with ID {job.Id}");

            return await GetJobByIdAsync(job.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating job: {ex.Message}");
            return null;
        }
    }

    public async Task<JobResponse?> UpdateJobAsync(Guid id, UpdateJobRequest request)
    {
        try
        {
            var job = await _dbContext.Jobs
                .Include(j => j.JobSkills)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
                return null;

            job.Title = request.Title;
            job.Description = request.Description;
            job.Location = request.Location;
            job.SalaryRange = request.SalaryRange;

            // Update skills
            if (request.RequiredSkillIds != null)
            {
                _dbContext.JobSkills.RemoveRange(job.JobSkills);

                foreach (var skillId in request.RequiredSkillIds)
                {
                    _dbContext.JobSkills.Add(new JobSkill { JobId = job.Id, SkillId = skillId });
                }
            }

            _dbContext.Jobs.Update(job);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Job {id} updated");

            return await GetJobByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating job {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> CloseJobAsync(Guid id)
    {
        try
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == id);
            if (job == null)
                return false;

            job.IsClosed = true;
            _dbContext.Jobs.Update(job);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Job {id} closed");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error closing job {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteJobAsync(Guid id)
    {
        try
        {
            var job = await _dbContext.Jobs.FirstOrDefaultAsync(j => j.Id == id);
            if (job == null)
                return false;

            _dbContext.Jobs.Remove(job);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Job {id} deleted");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting job {id}: {ex.Message}");
            return false;
        }
    }

    private static JobResponse MapToJobResponse(Data.Entities.Job job)
    {
        return new JobResponse
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Location = job.Location,
            SalaryRange = job.SalaryRange,
            IsClosed = job.IsClosed,
            CompanyId = job.CompanyId,
            CompanyName = job.Company?.Name,
            CreatedAt = job.CreatedAt,
            ApplicationCount = job.Applications?.Count ?? 0,
            RequiredSkills = job.JobSkills?.Select(js => new SkillResponse
            {
                Id = js.Skill.Id,
                Name = js.Skill.Name
            }).ToList()
        };
    }
}
