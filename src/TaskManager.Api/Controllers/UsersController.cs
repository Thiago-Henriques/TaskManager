using Microsoft.AspNetCore.Mvc;
using TaskManager.Domain.Entities;
using TaskManager.Application.Interfaces;

namespace TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(id.ToString()); // Se quiser buscar por ID, crie método adequado
                if (user == null) return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(User user)
        {
            try
            {
                await _userService.AddUserAsync(user);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login(string email, string password)
        {
            try
            {
                var user = await _userService.LoginAsync(email, password);
                if (user == null)
                    return Unauthorized("Invalid credentials");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
