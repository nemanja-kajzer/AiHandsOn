using AiHandsOn.Api.Dtos;
using System.Collections.Generic;

namespace AiHandsOn.Api.Validation
{
    // Responsibility: validate incoming RegisterUserDto payloads
    // Returns a list of error messages; empty list means valid
    public interface IRegistrationValidator
    {
        List<string> Validate(RegisterUserDto dto);
    }
}