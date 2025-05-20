using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using backend.Data.Repositories;
using backend.Models;
using backend.Models.DTOs;

namespace backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        
        public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task<User> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
                
                var user = new User
                {
                    Username = registerDto.Username,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                };
                
                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
                
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterAsync for user: {Username}", registerDto.Username);
                throw;
            }
        }
        
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);
                
                if (user == null)
                    return null;
                    
                if (!VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
                    return null;
                    
                return CreateToken(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LoginAsync for user: {Username}", loginDto.Username);
                throw;
            }
        }
        
        public async Task<bool> UserExistsAsync(string username)
        {
            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                return user != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UserExistsAsync for username: {Username}", username);
                throw;
            }
        }
        
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                        return false;
                }
                
                return true;
            }
        }
        
        private string CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));
                
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return tokenHandler.WriteToken(token);
        }
    }
}