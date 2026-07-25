namespace HireSphere.Api.Features.Application.Dtos;

public class CreateApplicationRequest
{
    public Guid JobId { get; set; }
    public Guid CandidateProfileId { get; set; }
    public string? CoverLetter { get; set; }
}

public class UpdateApplicationStatusRequest
{
    public string Status { get; set; } = string.Empty; // Submitted, Shortlisted, Interviewing, Accepted, Rejected
}

public class ApplicationResponse
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string? JobTitle { get; set; }
    public Guid CandidateProfileId { get; set; }
    public string? CandidateName { get; set; }
    public string? CoverLetter { get; set; }
    public string Status { get; set; } = string.Empty;
    public double MatchScore { get; set; }
    public string? MatchExplanation { get; set; }
    public DateTime CreatedAt { get; set; }
    public int InterviewCount { get; set; }
}

public class ApplicationListResponse
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string? JobTitle { get; set; }
    public string? CandidateName { get; set; }
    public string Status { get; set; } = string.Empty;
    public double MatchScore { get; set; }
    public DateTime CreatedAt { get; set; }
}
