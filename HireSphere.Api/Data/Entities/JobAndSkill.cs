using System.ComponentModel.DataAnnotations;

namespace HireSphere.Api.Data.Entities;

public class Job
{
    public Guid Id { get; set; }
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? SalaryRange { get; set; }
    public bool IsClosed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
}

public class Skill
{
    public Guid Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
}

public class JobSkill
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
}

public class CandidateSkill
{
    public Guid CandidateProfileId { get; set; }
    public CandidateProfile CandidateProfile { get; set; } = null!;
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
}
