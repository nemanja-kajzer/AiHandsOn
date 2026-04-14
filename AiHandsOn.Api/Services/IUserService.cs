using AiHandsOn.Api.Dtos;

namespace AiHandsOn.Api.Services
{
    public interface IUserService
    {
        Task<RegistrationResult> RegisterAsync(RegisterUserDto dto);
    }
}