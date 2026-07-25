using System.ComponentModel.DataAnnotations;

namespace HireSphere.Api.Data.Entities;

public class Company
{
    public Guid Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    public ICollection<RecruiterProfile> Recruiters { get; set; } = new List<RecruiterProfile>();
}
