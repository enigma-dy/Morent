using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using MoRent_V2.Models;

namespace MoRent_Server.Models;

public class Rental
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime RentalStartDate { get; set; }

    [Required]
    public DateTime RentalEndDate { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal TotalCost { get; set; }

    public required string CustomerId { get; set; } = string.Empty;

    public MoRentUser Customer { get; set; } = null!;


    public int CarId { get; set; }
    public Car Car { get; set; } = null!;
}