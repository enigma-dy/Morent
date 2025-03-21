using Microsoft.AspNetCore.Identity;
using MoRent_Server.Models;
using System.ComponentModel.DataAnnotations;

namespace MoRent_V2.Models
{
    public class MoRentUser : IdentityUser
    {
        [Required, MaxLength(60)]
        public required string FullName { get; set; }

        public string ProfilePics { get; set; } = string.Empty;

        public bool IsDealer { get; set; }

        public bool IsVerifiedDealer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Car> ListedCars { get; set; } = [];

        public ICollection<Rental> Rentals { get; set; } = [];


    }
}