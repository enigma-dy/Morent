using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoRent_V2.Models;
using MoRent_V2.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoRent_V2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    UserManager<MoRentUser> userManager,
    SignInManager<MoRentUser> signInManager,
    IConfiguration configuration,
    IMapper mapper) : ControllerBase
{
    private readonly UserManager<MoRentUser> _userManager = userManager;
    private readonly SignInManager<MoRentUser> _signInManager = signInManager;
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;

    // Register a new user
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Register model, [FromServices] EmailService emailService)
    {
        var user = _mapper.Map<MoRentUser>(model);
        var result = await _userManager.CreateAsync(user, model.Password!);

        if (result.Succeeded)
        {

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "User", new { userId = user.Id, token }, Request.Scheme);



            await emailService.SendEmailAsync(user.Email ?? throw new Exception("Enter a valid email address"), "Confirm your email", $"Please confirm your email by clicking this link: {confirmationLink}");

            return Ok("User registered successfully. Please check your email to confirm your account.");
        }

        return BadRequest(result.Errors);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email!);
        if (user == null)
        {
            return Unauthorized("Invalid login attempt.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password!, false);
        if (!result.Succeeded)
        {
            return Unauthorized("Invalid login attempt.");
        }

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    // Generate JWT token
    private string GenerateJwtToken(MoRentUser user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]!);

        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Email, user.Email!),
        new(ClaimTypes.Name, user.FullName)
    };


        if (user.IsDealer)
        {
            claims.Add(new Claim("IsDealer", "true"));
        }
        if (user.IsVerifiedDealer)
        {
            claims.Add(new Claim("IsVerifiedDealer", "true"));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        _ = _mapper.Map<MoRentUser>(user);

        return Ok(user);
    }
}