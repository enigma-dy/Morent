using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoRent_V2.Models;

public class SponsorAd
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Description is required.")]
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    public required string Description { get; set; }

    [Required(ErrorMessage = "At least one image is required.")]
    public List<string> Images { get; set; } = [];

    [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "Sponsor ID is required.")]
    public required string SponsorId { get; set; }

    [ForeignKey("SponsorId")]
    public MoRentUser Sponsor { get; set; } = null!;

    [Required(ErrorMessage = "Car ID is required.")]
    public int CarId { get; set; }

    [ForeignKey("CarId")]
    public required Car Car { get; set; }
}