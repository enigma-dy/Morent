using System;
using System.ComponentModel.DataAnnotations;

namespace MoRent_V2.Models.Dto;

public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Token { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string? ConfirmPassword { get; set; }
}