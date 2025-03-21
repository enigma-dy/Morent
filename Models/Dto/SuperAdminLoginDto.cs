using System;

namespace MoRent_V2.Models.Dto;

public class SuperAdminLoginDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
