using System;
using System.ComponentModel.DataAnnotations;

namespace MoRent_V2.Models;


public class Register
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    public required string Password { get; set; }
    public required string FullName { get; set; }
}