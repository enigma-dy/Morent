using System;
using System.ComponentModel.DataAnnotations;

namespace MoRent_V2.Models.Dto;

public class ChangeEmailDto
{
    [Required]
    [EmailAddress]
    public required string CurrentEmail { get; set; }

    [Required]
    [EmailAddress]
    public required string NewEmail { get; set; }

    [Required]
    public required string Token { get; set; }
}