using Microsoft.AspNetCore.Mvc;
using HireSphere.Api.Common;
using HireSphere.Api.Features.Auth.Dtos;
using HireSphere.Api.Features.Auth.Services;

namespace HireSphere.Api.Features.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenService _tokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IJwtTokenService tokenService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(ApiResponse<LoginResponse>.FailureResponse("Invalid input", errors));
        }

        var result = await _authService.LoginAsync(request);
        if (result == null)
        {
            _logger.LogWarning($"Failed login attempt for user: {request.Username}");
            return Unauthorized(ApiResponse<LoginResponse>.FailureResponse("Invalid username or password"));
        }

        _logger.LogInformation($"User {request.Username} logged in successfully");
        return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Login successful"));
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<RegisterResponse>>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
            return BadRequest(ApiResponse<RegisterResponse>.FailureResponse("Invalid input", errors));
        }

        if (request.Role != "Candidate" && request.Role != "Recruiter")
        {
            return BadRequest(ApiResponse<RegisterResponse>.FailureResponse("Role must be 'Candidate' or 'Recruiter'"));
        }

        var result = await _authService.RegisterAsync(request);
        if (result == null)
        {
            _logger.LogWarning($"Failed registration attempt for user: {request.Username}");
            return BadRequest(ApiResponse<RegisterResponse>.FailureResponse("Registration failed. Username or email may already exist"));
        }

        _logger.LogInformation($"User {request.Username} registered successfully as {request.Role}");
        return CreatedAtAction(nameof(Register), ApiResponse<RegisterResponse>.SuccessResponse(result, "Registration successful"));
    }

    [HttpPost("verify-token")]
    public async Task<ActionResult<ApiResponse<VerifyTokenResponse>>> VerifyToken([FromBody] VerifyTokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return BadRequest(ApiResponse<VerifyTokenResponse>.FailureResponse("Token is required"));
        }

        var result = await _authService.VerifyTokenAsync(request.Token, _tokenService);
        return Ok(ApiResponse<VerifyTokenResponse>.SuccessResponse(result, "Token verification completed"));
    }
}
