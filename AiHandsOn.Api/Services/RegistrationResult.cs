namespace AiHandsOn.Api.Services
{
    public class RegistrationResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public Dtos.UserDto? User { get; init; }

        public static RegistrationResult Ok() => new RegistrationResult { Success = true };
        public static RegistrationResult Ok(Dtos.UserDto user) => new RegistrationResult { Success = true, User = user };
        public static RegistrationResult Fail(string message) => new RegistrationResult { Success = false, ErrorMessage = message };
    }
}