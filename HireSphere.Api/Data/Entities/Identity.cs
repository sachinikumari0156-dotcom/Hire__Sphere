using System.ComponentModel.DataAnnotations;

namespace HireSphere.Api.Data.Entities;

public class User
{
    public Guid Id { get; set; }
    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public CandidateProfile? CandidateProfile { get; set; }
    public RecruiterProfile? RecruiterProfile { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}

public class Role
{
    public Guid Id { get; set; }
    [Required, MaxLength(20)]
    public string Name { get; set; } = string.Empty;
    public ICollection<User> Users { get; set; } = new List<User>();
}
