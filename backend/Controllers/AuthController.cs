using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using backend.Models.DTOs;
using backend.Services;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Registration attempt for user: {Username}", registerDto.Username);
                
                if (string.IsNullOrEmpty(registerDto.Username) || 
                    string.IsNullOrEmpty(registerDto.Email) || 
                    string.IsNullOrEmpty(registerDto.Password))
                {
                    _logger.LogWarning("Registration failed: Required fields missing");
                    return BadRequest(new { message = "Username, email, and password are required" });
                }
                
                if (await _authService.UserExistsAsync(registerDto.Username))
                {
                    _logger.LogWarning("Registration failed: Username {Username} already exists", registerDto.Username);
                    return BadRequest(new { message = "Username already exists" });
                }
                
                var user = await _authService.RegisterAsync(registerDto);
                
                _logger.LogInformation("User registered successfully: {Username}", registerDto.Username);
                
                return Ok(new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.FirstName,
                    user.LastName
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user: {Username}", registerDto.Username);
                return BadRequest(new { message = "Registration failed: " + ex.Message });
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt for user: {Username}", loginDto.Username);
                
                var token = await _authService.LoginAsync(loginDto);
                
                if (token == null)
                {
                    _logger.LogWarning("Login failed for user: {Username}", loginDto.Username);
                    return Unauthorized(new { message = "Invalid username or password" });
                }
                
                _logger.LogInformation("User logged in successfully: {Username}", loginDto.Username);
                
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", loginDto.Username);
                return BadRequest(new { message = "Login failed: " + ex.Message });
            }
        }
    }
}