using AiHandsOn.Api.Dtos;
using AiHandsOn.Api.Services;
using AiHandsOn.Api.Validation;
using Microsoft.AspNetCore.Mvc;

namespace AiHandsOn.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserControllerManual : ControllerBase
    {
        private readonly IUserServiceManual _userService;
        private readonly IRegistrationValidator _registrationValidator;

        public UserControllerManual(IUserServiceManual service, IRegistrationValidator validator)
        {
            _userService = service;
            _registrationValidator = validator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterManual([FromBody] RegisterUserDto dto)
        {
            var errors = _registrationValidator.Validate(dto);

            if (errors.Count > 0)
            {
                // Return 400 with a list of validation errors
                return StatusCode(400, new { errors });
            }

            var result = await _userService.RegisterAsync(dto);
            if (!result.Success)
            {
                return StatusCode(409, new { error = result.ErrorMessage });
            }

            return StatusCode(201, result.User);
        }


    }
}