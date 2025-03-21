using System;
using System.ComponentModel.DataAnnotations;

namespace MoRent_V2.Models.Dto;

public class CreatPrevilageUserDto
{
    [Required]
    public required string Email { get; set; }
    public required string FullName { get; set; }
    [Required]
    public required string Password { get; set; }
    [Required]
    public required string Role { get; set; }



}
