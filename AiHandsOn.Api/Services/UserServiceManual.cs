using AiHandsOn.Api.Dtos;
using AiHandsOn.Api.Models;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AiHandsOn.Api.Services
{
    // Manual implementation that follows the same contract but written by hand
    public class UserServiceManual : IUserServiceManual
    {
        private readonly ConcurrentDictionary<string, User> _store = new();

        public Task<RegistrationResult> RegisterAsync(RegisterUserDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var normalizedEmail = NormalizeEmail(dto.Email);

            if (_store.ContainsKey(normalizedEmail))
            {
                return Task.FromResult(RegistrationResult.Fail("User with this email already exists."));
            }

            var salt = GenerateSalt(16);
            var hash = ComputeHash(dto.Password, salt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordSalt = Convert.ToBase64String(salt),
                PasswordHash = Convert.ToBase64String(hash),
                CreatedAt = DateTimeOffset.UtcNow
            };

            var added = _store.TryAdd(normalizedEmail, user);
            if (!added)
            {
                return Task.FromResult(RegistrationResult.Fail("Could not register user. Try again."));
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

        private static string NormalizeEmail(string email) => email?.Trim().ToLowerInvariant() ?? string.Empty;

        private static byte[] GenerateSalt(int size)
        {
            var bytes = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return bytes;
        }

        private static byte[] ComputeHash(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            return pbkdf2.GetBytes(32);
        }
    }
}