using Microsoft.EntityFrameworkCore;
using HireSphere.Api.Data.Entities;
using System.Security.Cryptography;
using System.Text;

namespace HireSphere.Api.Data;

public static class DatabaseSeeder
{
    public static void Seed(HireSphereDbContext context)
    {
        context.Database.EnsureCreated();

        // Seed Roles if they don't exist
        if (!context.Roles.Any())
        {
            var adminRole = new Role { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Admin" };
            var candidateRole = new Role { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Candidate" };
            var recruiterRole = new Role { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Recruiter" };

            context.Roles.AddRange(adminRole, candidateRole, recruiterRole);
            context.SaveChanges();
        }

        // Seed Skills if they don't exist
        if (!context.Skills.Any())
        {
            var skills = new List<Skill>
            {
                new() { Id = Guid.NewGuid(), Name = "C#" },
                new() { Id = Guid.NewGuid(), Name = "React" },
                new() { Id = Guid.NewGuid(), Name = "TypeScript" },
                new() { Id = Guid.NewGuid(), Name = "Python" },
                new() { Id = Guid.NewGuid(), Name = "SQL" },
                new() { Id = Guid.NewGuid(), Name = "Machine Learning" },
                new() { Id = Guid.NewGuid(), Name = "Azure" }
            };

            context.Skills.AddRange(skills);
            context.SaveChanges();
        }

        // Seed Default Admin User if not exists
        if (!context.Users.Any(u => u.Username == "admin"))
        {
            var adminUser = new User
            {
                Id = Guid.Parse("a0a0a0a0-a0a0-a0a0-a0a0-a0a0a0a0a0a0"),
                Username = "admin",
                Email = "admin@hiresphere.com",
                PasswordHash = HashPassword("Admin123!"),
                RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(adminUser);
            context.SaveChanges();
        }

        // Seed some sample data (Company, Recruiter, Candidate)
        if (!context.Companies.Any())
        {
            var company = new Company
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Name = "HireSphere Tech",
                Description = "Innovative software solutions company specializing in AI recruitment systems.",
                Website = "https://hiresphere.com",
                Location = "San Francisco, CA"
            };
            context.Companies.Add(company);
            context.SaveChanges();

            // Recruiter User & Profile
            var recruiterUser = new User
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Username = "recruiter",
                Email = "recruiter@hiresphere.com",
                PasswordHash = HashPassword("Recruiter123!"),
                RoleId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(recruiterUser);
            context.SaveChanges();

            var recruiterProfile = new RecruiterProfile
            {
                Id = Guid.NewGuid(),
                UserId = recruiterUser.Id,
                CompanyId = company.Id,
                FullName = "Sarah Connor",
                CreatedAt = DateTime.UtcNow
            };
            context.RecruiterProfiles.Add(recruiterProfile);
            context.SaveChanges();

            // Candidate User & Profile
            var candidateUser = new User
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                Username = "candidate",
                Email = "candidate@hiresphere.com",
                PasswordHash = HashPassword("Candidate123!"),
                RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(candidateUser);
            context.SaveChanges();

            var candidateProfile = new CandidateProfile
            {
                Id = Guid.Parse("b0b0b0b0-b0b0-b0b0-b0b0-b0b0b0b0b0b0"),
                UserId = candidateUser.Id,
                FullName = "John Doe",
                Title = "Full Stack Engineer",
                Bio = "Experienced .NET & React developer passionate about AI tools and high scalability.",
                CreatedAt = DateTime.UtcNow
            };
            context.CandidateProfiles.Add(candidateProfile);
            context.SaveChanges();

            // Add Candidate Skills
            var reactSkill = context.Skills.FirstOrDefault(s => s.Name == "React");
            var csharpSkill = context.Skills.FirstOrDefault(s => s.Name == "C#");
            if (reactSkill != null)
            {
                context.CandidateSkills.Add(new CandidateSkill { CandidateProfileId = candidateProfile.Id, SkillId = reactSkill.Id });
            }
            if (csharpSkill != null)
            {
                context.CandidateSkills.Add(new CandidateSkill { CandidateProfileId = candidateProfile.Id, SkillId = csharpSkill.Id });
            }

            // Seed a sample job
            var job = new Job
            {
                Id = Guid.Parse("c0c0c0c0-c0c0-c0c0-c0c0-c0c0c0c0c0c0"),
                Title = "Senior Full Stack .NET & React Developer",
                Description = "Join HireSphere Tech to work on next-generation AI hiring platforms. Requirements: 5+ years with C#, ASP.NET Core, and React.",
                Location = "Remote (US)",
                SalaryRange = "$120,000 - $150,000",
                IsClosed = false,
                CompanyId = company.Id,
                CreatedAt = DateTime.UtcNow
            };
            context.Jobs.Add(job);
            context.SaveChanges();

            if (reactSkill != null)
            {
                context.JobSkills.Add(new JobSkill { JobId = job.Id, SkillId = reactSkill.Id });
            }
            if (csharpSkill != null)
            {
                context.JobSkills.Add(new JobSkill { JobId = job.Id, SkillId = csharpSkill.Id });
            }
            context.SaveChanges();
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
