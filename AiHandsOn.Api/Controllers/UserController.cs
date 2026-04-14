using AiHandsOn.Api.Dtos;
using AiHandsOn.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiHandsOn.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Collect all validation errors from ModelState and return them in a flat list
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? (e.Exception?.Message ?? string.Empty) : e.ErrorMessage)
                    .Where(msg => !string.IsNullOrWhiteSpace(msg))
                    .ToList();

                return BadRequest(new { errors });
            }

            var result = await _userService.RegisterAsync(dto);
            if (!result.Success)
            {
                return Conflict(new { message = result.ErrorMessage });
            }

            // Return created user DTO in response body. In a real app provide a route name and location.
            return Created(string.Empty, result.User);
        }
    }
}
