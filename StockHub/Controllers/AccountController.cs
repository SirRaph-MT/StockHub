using Core.Db;
using Core.DTOs;
using Logic.IHelper;
using Microsoft.AspNetCore.Mvc;

namespace StockHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUserHelper _userHelper;
        public AccountController(AppDbContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] ApplicationUserDTO dto)
        {
            if (ModelState.IsValid)
            {
                if (dto.FirstName == "string" || string.IsNullOrEmpty(dto.FirstName))
                    return BadRequest(new { error = "Please enter your first name" });
                if (dto.LastName == "string" || string.IsNullOrEmpty(dto.LastName))
                    return BadRequest(new { error = "Please enter your last name" });
                if (dto.Email == "string" || string.IsNullOrEmpty(dto.Email))
                    return BadRequest(new { error = "Please enter your email" });
                if (dto.Password != dto.ConfirmPassword)
                    return BadRequest(new { error = "Password and confirm password must match" });
                var existingUser = _context.ApplicationUsers.FirstOrDefault(u => u.Email == dto.Email);
                if (existingUser != null)
                    return BadRequest(new { error = "This email is already registered" });
                var result = await _userHelper.Registration(dto);
                if (result != null)
                    return Ok(new { message = "User registered successfully" });
            }
            return BadRequest(new { error = "An error occurred during registration" });
        }
    }
}
