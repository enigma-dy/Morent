using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MoRent_Server.Models;

namespace MoRent_V2.Models;

public class Car
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public required string Make { get; set; }

    [Required, MaxLength(100)]
    public required string Model { get; set; }

    [Required]
    public List<string> Pictures { get; set; } = [];

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Total quantity must be at least 1.")]
    public int TotalQuantity { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Available for sale must be a non-negative number.")]
    public int AvailableForSale { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Available for rent must be a non-negative number.")]
    public int AvailableForRent { get; set; }

    [Required]
    [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100.")]
    public int Year { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Required]
    public bool IsForSale { get; set; }

    [Required]
    public bool IsForRent { get; set; }

    [Required]
    public required string DealerId { get; set; }

    [ForeignKey("DealerId")]
    public MoRentUser Dealer { get; set; } = null!;

    public ICollection<Rental> Rentals { get; set; } = [];
}