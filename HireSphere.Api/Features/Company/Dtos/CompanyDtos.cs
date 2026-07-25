namespace HireSphere.Api.Features.Company.Dtos;

public class CreateCompanyRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Location { get; set; }
}

public class UpdateCompanyRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Location { get; set; }
}

public class CompanyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public int JobCount { get; set; }
    public int RecruiterCount { get; set; }
}

public class CompanyListResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
}
