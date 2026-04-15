namespace OrderManagement.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Api.Models;
using OrderManagement.Api.Services;

[ApiController]
[Route("[controller]")]
public sealed class OrdersController(OrderService service, TokenService tokenService) : ControllerBase
{
    private readonly OrderService _service = service;
    private readonly TokenService _tokenService = tokenService;

    [HttpPost("token")]
    public IActionResult CreateToken([FromQuery] string user = "demo") => Ok(new { token = _tokenService.GenerateToken(user) });

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var order = await _service.GetByIdAsync(id);
        if (order is null) return NotFound();
        return Ok(order);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] Order order)
    {
        var created = await _service.CreateAsync(order);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
