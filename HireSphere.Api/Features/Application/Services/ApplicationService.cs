using HireSphere.Api.Data;
using HireSphere.Api.Data.Entities;
using HireSphere.Api.Features.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HireSphere.Api.Features.Application.Services;

public interface IApplicationService
{
    Task<ApplicationResponse?> GetApplicationByIdAsync(Guid id);
    Task<List<ApplicationListResponse>> GetApplicationsByJobAsync(Guid jobId);
    Task<List<ApplicationListResponse>> GetApplicationsByCandidateAsync(Guid candidateProfileId);
    Task<ApplicationResponse?> CreateApplicationAsync(CreateApplicationRequest request);
    Task<ApplicationResponse?> UpdateApplicationStatusAsync(Guid id, UpdateApplicationStatusRequest request);
    Task<bool> WithdrawApplicationAsync(Guid id);
    Task<bool> DeleteApplicationAsync(Guid id);
}

public class ApplicationService : IApplicationService
{
    private readonly HireSphereDbContext _dbContext;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(HireSphereDbContext dbContext, ILogger<ApplicationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ApplicationResponse?> GetApplicationByIdAsync(Guid id)
    {
        try
        {
            var application = await _dbContext.Applications
                .Include(a => a.Job)
                .Include(a => a.CandidateProfile)
                .Include(a => a.Interviews)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return null;

            return MapToApplicationResponse(application);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving application {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<ApplicationListResponse>> GetApplicationsByJobAsync(Guid jobId)
    {
        try
        {
            var applications = await _dbContext.Applications
                .Include(a => a.Job)
                .Include(a => a.CandidateProfile)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.MatchScore)
                .ThenByDescending(a => a.CreatedAt)
                .Select(a => new ApplicationListResponse
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job.Title,
                    CandidateName = a.CandidateProfile.FullName,
                    Status = a.Status,
                    MatchScore = a.MatchScore,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return applications;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving applications for job {jobId}: {ex.Message}");
            return new List<ApplicationListResponse>();
        }
    }

    public async Task<List<ApplicationListResponse>> GetApplicationsByCandidateAsync(Guid candidateProfileId)
    {
        try
        {
            var applications = await _dbContext.Applications
                .Include(a => a.Job)
                .Where(a => a.CandidateProfileId == candidateProfileId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new ApplicationListResponse
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job.Title,
                    CandidateName = a.CandidateProfile.FullName,
                    Status = a.Status,
                    MatchScore = a.MatchScore,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return applications;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving applications for candidate {candidateProfileId}: {ex.Message}");
            return new List<ApplicationListResponse>();
        }
    }

    public async Task<ApplicationResponse?> CreateApplicationAsync(CreateApplicationRequest request)
    {
        try
        {
            // Check if application already exists
            var existing = await _dbContext.Applications
                .FirstOrDefaultAsync(a => a.JobId == request.JobId && a.CandidateProfileId == request.CandidateProfileId);

            if (existing != null)
            {
                _logger.LogWarning($"Application already exists for job {request.JobId} and candidate {request.CandidateProfileId}");
                return null;
            }

            var application = new Data.Entities.Application
            {
                Id = Guid.NewGuid(),
                JobId = request.JobId,
                CandidateProfileId = request.CandidateProfileId,
                CoverLetter = request.CoverLetter,
                Status = "Submitted",
                MatchScore = 0.0,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Applications.Add(application);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Application {application.Id} created for job {request.JobId}");

            return await GetApplicationByIdAsync(application.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating application: {ex.Message}");
            return null;
        }
    }

    public async Task<ApplicationResponse?> UpdateApplicationStatusAsync(Guid id, UpdateApplicationStatusRequest request)
    {
        try
        {
            var application = await _dbContext.Applications
                .Include(a => a.Job)
                .Include(a => a.CandidateProfile)
                .Include(a => a.Interviews)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return null;

            var validStatuses = new[] { "Submitted", "Shortlisted", "Interviewing", "Accepted", "Rejected" };
            if (!validStatuses.Contains(request.Status))
            {
                _logger.LogWarning($"Invalid status: {request.Status}");
                return null;
            }

            application.Status = request.Status;
            _dbContext.Applications.Update(application);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Application {id} status updated to {request.Status}");

            return MapToApplicationResponse(application);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating application {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> WithdrawApplicationAsync(Guid id)
    {
        try
        {
            var application = await _dbContext.Applications.FirstOrDefaultAsync(a => a.Id == id);
            if (application == null)
                return false;

            _dbContext.Applications.Remove(application);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Application {id} withdrawn");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error withdrawing application {id}: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteApplicationAsync(Guid id)
    {
        try
        {
            var application = await _dbContext.Applications.FirstOrDefaultAsync(a => a.Id == id);
            if (application == null)
                return false;

            _dbContext.Applications.Remove(application);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Application {id} deleted");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting application {id}: {ex.Message}");
            return false;
        }
    }

    private static ApplicationResponse MapToApplicationResponse(Data.Entities.Application application)
    {
        return new ApplicationResponse
        {
            Id = application.Id,
            JobId = application.JobId,
            JobTitle = application.Job?.Title,
            CandidateProfileId = application.CandidateProfileId,
            CandidateName = application.CandidateProfile?.FullName,
            CoverLetter = application.CoverLetter,
            Status = application.Status,
            MatchScore = application.MatchScore,
            MatchExplanation = application.MatchExplanation,
            CreatedAt = application.CreatedAt,
            InterviewCount = application.Interviews?.Count ?? 0
        };
    }
}
