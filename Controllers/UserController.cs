using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoRent_V2.Models;
using MoRent_V2.Models.Dto;
using MoRent_V2.Services;
using System.Security.Claims;

namespace MoRent_V2.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserController(UserManager<MoRentUser> userManager, CloudinaryService cloudinaryService, IMapper mapper, EmailService emailService) : ControllerBase
{
    private readonly UserManager<MoRentUser> _userManager = userManager;
    private readonly IMapper _mapper = mapper;
    private readonly CloudinaryService _cloudinaryService = cloudinaryService;
    private readonly EmailService _emailService = emailService;

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        _mapper.Map(model, user);


        if (model.ProfileImage != null)
        {

            user.ProfilePics = await _cloudinaryService.UploadImageAsync(model.ProfileImage);
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Ok("Profile updated successfully.");
        }

        return BadRequest(result.Errors);
    }
    // Delete user profile
    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return Ok("Profile deleted successfully.");
        }

        return BadRequest(result.Errors);
    }

    [AllowAnonymous]
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            return Ok("Email confirmed successfully.");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
        if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
        {

            return Ok("If your email is registered, you will receive a password reset link.");
        }


        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action("ResetPassword", "User", new { email = user.Email, token }, Request.Scheme);

        await _emailService.SendEmailAsync(user.Email ?? throw new Exception("Email doesnt exist or is not verified"), "Reset your password", $"Please reset your password by clicking this link: {resetLink}");

        return Ok("Password reset link sent to your email.");
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

        if (result.Succeeded)
        {
            return Ok("Password reset successfully.");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("change-email")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailDto changeEmailDto)
    {
        var user = await _userManager.FindByEmailAsync(changeEmailDto.CurrentEmail);
        if (user == null)
        {
            return NotFound("User not found.");
        }


        var token = await _userManager.GenerateChangeEmailTokenAsync(user, changeEmailDto.NewEmail);


        var confirmationLink = Url.Action("ConfirmChangeEmail", "User", new { userId = user.Id, newEmail = changeEmailDto.NewEmail, token }, Request.Scheme);

        await _emailService.SendEmailAsync(changeEmailDto.NewEmail, "Confirm your new email", $"Please confirm your new email by clicking this link: {confirmationLink}");   // SendEmail(changeEmailDto.NewEmail, "Confirm your new email", $"Please confirm your new email by clicking this link: {confirmationLink}");

        return Ok("Confirmation link sent to your new email.");
    }

    [HttpPost("enable-2fa")]
    public async Task<IActionResult> EnableTwoFactorAuthentication()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound("User ID not found.");
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
        if (result.Succeeded)
        {
            return Ok("Two-factor authentication enabled successfully.");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("generate-2fa-token")]
    public async Task<IActionResult> GenerateTwoFactorToken()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound("User ID not found.");
        }
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email"); // or "Phone"
                                                                                   // Send the token to the user's email or phone
                                                                                   // SendEmail(user.Email, "Your 2FA Code", $"Your 2FA code is: {token}");

        return Ok("2FA token generated and sent.");
    }

    [HttpPost("setup-authenticator")]
    public async Task<IActionResult> SetupAuthenticator()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound("User ID not found.");
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }


        var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(authenticatorKey))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var qrCodeUri = $"otpauth://totp/{user.Email}?secret={authenticatorKey}&issuer=MoRent";

        return Ok(new { AuthenticatorKey = authenticatorKey, QrCodeUri = qrCodeUri });
    }

    [HttpPost("verify-authenticator")]
    public async Task<IActionResult> VerifyAuthenticator([FromBody] string code)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return NotFound("User ID not found.");
        }
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, "Authenticator", code);
        if (isValid)
        {
            return Ok("Authenticator code verified successfully.");
        }

        return BadRequest("Invalid authenticator code.");
    }
}