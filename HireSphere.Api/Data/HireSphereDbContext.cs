using Microsoft.EntityFrameworkCore;
using HireSphere.Api.Data.Entities;

namespace HireSphere.Api.Data;

public class HireSphereDbContext : DbContext
{
    public HireSphereDbContext(DbContextOptions<HireSphereDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Company> Companies { get; set; } = null!;
    public DbSet<CandidateProfile> CandidateProfiles { get; set; } = null!;
    public DbSet<RecruiterProfile> RecruiterProfiles { get; set; } = null!;
    public DbSet<Job> Jobs { get; set; } = null!;
    public DbSet<Skill> Skills { get; set; } = null!;
    public DbSet<JobSkill> JobSkills { get; set; } = null!;
    public DbSet<CandidateSkill> CandidateSkills { get; set; } = null!;
    public DbSet<Application> Applications { get; set; } = null!;
    public DbSet<Resume> Resumes { get; set; } = null!;
    public DbSet<Interview> Interviews { get; set; } = null!;
    public DbSet<InterviewFeedback> InterviewFeedbacks { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<AuditLog> AuditLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User & Role
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // One-to-one between User and Profiles
        modelBuilder.Entity<CandidateProfile>()
            .HasOne(cp => cp.User)
            .WithOne(u => u.CandidateProfile)
            .HasForeignKey<CandidateProfile>(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RecruiterProfile>()
            .HasOne(rp => rp.User)
            .WithOne(u => u.RecruiterProfile)
            .HasForeignKey<RecruiterProfile>(rp => rp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // RecruiterProfile & Company
        modelBuilder.Entity<RecruiterProfile>()
            .HasOne(rp => rp.Company)
            .WithMany(c => c.Recruiters)
            .HasForeignKey(rp => rp.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Core Relationships:
        // 1. Company to Jobs
        modelBuilder.Entity<Job>()
            .HasOne(j => j.Company)
            .WithMany(c => c.Jobs)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        // 2. Candidate to Applications
        modelBuilder.Entity<Application>()
            .HasOne(a => a.CandidateProfile)
            .WithMany(cp => cp.Applications)
            .HasForeignKey(a => a.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // 3. Job to Applications
        modelBuilder.Entity<Application>()
            .HasOne(a => a.Job)
            .WithMany(j => j.Applications)
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        // 4. Application to Interview
        modelBuilder.Entity<Interview>()
            .HasOne(i => i.Application)
            .WithMany(a => a.Interviews)
            .HasForeignKey(i => i.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);

        // 5. Candidate to Skills (Many-to-Many via CandidateSkill)
        modelBuilder.Entity<CandidateSkill>()
            .HasKey(cs => new { cs.CandidateProfileId, cs.SkillId });

        modelBuilder.Entity<CandidateSkill>()
            .HasOne(cs => cs.CandidateProfile)
            .WithMany(cp => cp.CandidateSkills)
            .HasForeignKey(cs => cs.CandidateProfileId);

        modelBuilder.Entity<CandidateSkill>()
            .HasOne(cs => cs.Skill)
            .WithMany(s => s.CandidateSkills)
            .HasForeignKey(cs => cs.SkillId);

        // 6. Job to Required Skills (Many-to-Many via JobSkill)
        modelBuilder.Entity<JobSkill>()
            .HasKey(js => new { js.JobId, js.SkillId });

        modelBuilder.Entity<JobSkill>()
            .HasOne(js => js.Job)
            .WithMany(j => j.JobSkills)
            .HasForeignKey(js => js.JobId);

        modelBuilder.Entity<JobSkill>()
            .HasOne(js => js.Skill)
            .WithMany(s => s.JobSkills)
            .HasForeignKey(js => js.SkillId);

        // Resume & CandidateProfile
        modelBuilder.Entity<Resume>()
            .HasOne(r => r.CandidateProfile)
            .WithMany(cp => cp.Resumes)
            .HasForeignKey(r => r.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        // InterviewFeedback & Interview
        modelBuilder.Entity<InterviewFeedback>()
            .HasOne(ifb => ifb.Interview)
            .WithMany(i => i.Feedbacks)
            .HasForeignKey(ifb => ifb.InterviewId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notifications & AuditLog
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany(u => u.AuditLogs)
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
