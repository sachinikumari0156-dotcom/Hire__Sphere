using System.ComponentModel.DataAnnotations;

namespace HireSphere.Api.Data.Entities;

public class CandidateProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Bio { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}

public class RecruiterProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
