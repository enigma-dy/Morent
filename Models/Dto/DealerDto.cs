using System;

namespace MoRent_V2.Models.Dto;

public class DealerDto
{
    public string FullName { get; set; } = string.Empty;
    public bool IsVerifiedDealer { get; set; }
}