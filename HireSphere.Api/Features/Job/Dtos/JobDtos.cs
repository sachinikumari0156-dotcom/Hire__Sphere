namespace HireSphere.Api.Features.Job.Dtos;

public class CreateJobRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? SalaryRange { get; set; }
    public Guid CompanyId { get; set; }
    public List<Guid>? RequiredSkillIds { get; set; }
}

public class UpdateJobRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? SalaryRange { get; set; }
    public List<Guid>? RequiredSkillIds { get; set; }
}

public class JobResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? SalaryRange { get; set; }
    public bool IsClosed { get; set; }
    public Guid CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ApplicationCount { get; set; }
    public List<SkillResponse>? RequiredSkills { get; set; }
}

public class JobListResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? SalaryRange { get; set; }
    public Guid CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public bool IsClosed { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ApplicationCount { get; set; }
}

public class SkillResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
