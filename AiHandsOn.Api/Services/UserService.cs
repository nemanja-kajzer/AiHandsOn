using AiHandsOn.Api.Dtos;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AiHandsOn.Api.Services
{
    public class UserService : IUserService
    {
        // Simple in-memory store keyed by email
        private readonly ConcurrentDictionary<string, Models.User> _users = new();

        public Task<RegistrationResult> RegisterAsync(RegisterUserDto dto)
        {
            var emailKey = dto.Email.Trim().ToLowerInvariant();
            if (_users.ContainsKey(emailKey))
            {
                return Task.FromResult(RegistrationResult.Fail("A user with that email already exists."));
            }

            // Generate salt
            var saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            var salt = Convert.ToBase64String(saltBytes);

            // Hash password using PBKDF2
            var hash = HashPassword(dto.Password, saltBytes);

            var user = new Models.User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordSalt = salt,
                PasswordHash = Convert.ToBase64String(hash),
                CreatedAt = DateTimeOffset.UtcNow
            };

            var added = _users.TryAdd(emailKey, user);
            if (!added)
            {
                return Task.FromResult(RegistrationResult.Fail("Failed to add user. Please try again."));
            }

            var userDto = new Dtos.UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

            return Task.FromResult(RegistrationResult.Ok(userDto));
        }

        private static byte[] HashPassword(string password, byte[] saltBytes)
        {
            // 100k iterations recommended for PBKDF2
            using var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes, 100_000, HashAlgorithmName.SHA256);
            return deriveBytes.GetBytes(32); // 256-bit hash
        }
    }
}