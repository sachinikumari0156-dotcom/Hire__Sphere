using HireSphere.Api.Data;
using HireSphere.Api.Data.Entities;
using HireSphere.Api.Features.Auth.Dtos;
using System.Security.Cryptography;
using System.Text;

namespace HireSphere.Api.Features.Auth.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<RegisterResponse?> RegisterAsync(RegisterRequest request);
    Task<VerifyTokenResponse> VerifyTokenAsync(string token, IJwtTokenService tokenService);
}

public class AuthService : IAuthService
{
    private readonly HireSphereDbContext _dbContext;
    private readonly IJwtTokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(HireSphereDbContext dbContext, IJwtTokenService tokenService, ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == request.Username);
            if (user == null)
            {
                _logger.LogWarning($"Login failed: User {request.Username} not found");
                return null;
            }

            if (!VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning($"Login failed: Invalid password for user {request.Username}");
                return null;
            }

            var role = _dbContext.Roles.FirstOrDefault(r => r.Id == user.RoleId);
            if (role == null)
            {
                _logger.LogError($"Role not found for user {user.Id}");
                return null;
            }

            var token = _tokenService.GenerateToken(user, role);

            return new LoginResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = role.Name,
                Token = token
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login error: {ex.Message}");
            return null;
        }
    }

    public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            // Check if user already exists
            if (_dbContext.Users.Any(u => u.Username == request.Username || u.Email == request.Email))
            {
                _logger.LogWarning($"Registration failed: Username or email already exists");
                return null;
            }

            // Get role
            var role = _dbContext.Roles.FirstOrDefault(r => r.Name == request.Role);
            if (role == null)
            {
                _logger.LogWarning($"Registration failed: Role {request.Role} not found");
                return null;
            }

            // Create new user
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                RoleId = role.Id,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // If registering as Candidate or Recruiter, create corresponding profile
            if (request.Role == "Candidate")
            {
                var candidateProfile = new CandidateProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    FullName = request.Username,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.CandidateProfiles.Add(candidateProfile);
            }
            else if (request.Role == "Recruiter")
            {
                // For recruiter, they need to select/create a company separately
                _logger.LogInformation($"Recruiter {user.Username} registered. Company profile to be created separately.");
            }

            await _dbContext.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user, role);

            return new RegisterResponse
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Registration error: {ex.Message}");
            return null;
        }
    }

    public async Task<VerifyTokenResponse> VerifyTokenAsync(string token, IJwtTokenService tokenService)
    {
        try
        {
            var principal = tokenService.VerifyToken(token);
            if (principal == null)
            {
                return new VerifyTokenResponse { IsValid = false };
            }

            var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var usernameClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var roleClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return new VerifyTokenResponse
            {
                IsValid = true,
                UserId = Guid.TryParse(userIdClaim, out var userId) ? userId : null,
                Username = usernameClaim,
                Role = roleClaim
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Token verification error: {ex.Message}");
            return new VerifyTokenResponse { IsValid = false };
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}
