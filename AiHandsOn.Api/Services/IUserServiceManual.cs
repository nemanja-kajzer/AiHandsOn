using AiHandsOn.Api.Dtos;
using System.Threading.Tasks;

namespace AiHandsOn.Api.Services
{
    // Manual variant of the user service interface used for the course comparison
    public interface IUserServiceManual
    {
        // Registers a user and returns a RegistrationResult (reuses existing RegistrationResult type)
        Task<RegistrationResult> RegisterAsync(RegisterUserDto dto);
    }
}