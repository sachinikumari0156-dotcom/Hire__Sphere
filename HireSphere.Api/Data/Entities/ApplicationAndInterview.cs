using System.ComponentModel.DataAnnotations;

namespace HireSphere.Api.Data.Entities;

public class Application
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;
    public Guid CandidateProfileId { get; set; }
    public CandidateProfile CandidateProfile { get; set; } = null!;
    public string? CoverLetter { get; set; }
    [Required, MaxLength(50)]
    public string Status { get; set; } = "Submitted"; // e.g., Submitted, Shortlisted, Interviewing, Accepted, Rejected
    public double MatchScore { get; set; } = 0.0;
    public string? MatchExplanation { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Interview> Interviews { get; set; } = new List<Interview>();
}

public class Resume
{
    public Guid Id { get; set; }
    public Guid CandidateProfileId { get; set; }
    public CandidateProfile CandidateProfile { get; set; } = null!;
    [Required]
    public string FilePath { get; set; } = string.Empty;
    public string? ParsedText { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Interview
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public Application Application { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    [Required, MaxLength(50)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled
    public string? MeetingUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<InterviewFeedback> Feedbacks { get; set; } = new List<InterviewFeedback>();
}

public class InterviewFeedback
{
    public Guid Id { get; set; }
    public Guid InterviewId { get; set; }
    public Interview Interview { get; set; } = null!;
    public int Score { get; set; } // e.g. 1 to 5 or 1 to 10
    [Required]
    public string FeedbackComments { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
