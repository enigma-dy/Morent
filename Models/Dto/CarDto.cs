using System;
using System.Collections.Generic;

namespace MoRent_V2.Models.Dto;

public class CarDto
{
    public int Id { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public List<string> Pictures { get; set; } = new();
    public int TotalQuantity { get; set; }
    public int Year { get; set; }
    public decimal Price { get; set; }
    public int AvailableForSale { get; set; }
    public int AvailableForRent { get; set; }
    public bool IsForSale { get; set; }
    public bool IsForRent { get; set; }
    public DealerDto? Dealer { get; set; }
}
