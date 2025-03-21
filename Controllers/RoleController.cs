using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoRent_V2.Models;
using MoRent_V2.Models.Dto;


namespace MoRent_V2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController(RoleManager<IdentityRole> roleManager, UserManager<MoRentUser> userManager) : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly UserManager<MoRentUser> _userManager = userManager;

        [HttpPost("create-role")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role name is required.");
            }

            // Check if the role already exists
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                return BadRequest("Role already exists.");
            }

            // Create the new role
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                return Ok($"Role '{roleName}' created successfully.");
            }

            // If role creation fails, return the errors
            return BadRequest(result.Errors);
        }

        [HttpPost("create-user")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreatPrevilageUserDto createPrevilageUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new MoRentUser
            {
                UserName = createPrevilageUser.Email,
                Email = createPrevilageUser.Email,
                FullName = createPrevilageUser.FullName,
            };

            var result = await _userManager.CreateAsync(user, createPrevilageUser.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, createPrevilageUser.Role);
                return Ok($"User '{createPrevilageUser.Email}' created successfully with role '{createPrevilageUser.Role}'.");
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("verify-dealer/{userId}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> VerifyDealer(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (!await _userManager.IsInRoleAsync(user, "Dealer"))
            {
                return BadRequest("User is not a dealer.");
            }
            user.IsVerifiedDealer = true;
            await _userManager.UpdateAsync(user);

            return Ok("Dealer verified successfully.");
        }
    }
}