using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MoRent_Server.Models;
using MoRent_V2.Models;

namespace MoRent_V2.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<MoRentUser>(options)
{
    public DbSet<SponsorAd> SponsorAds { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<Rental> Rentals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Car>()
            .HasOne(c => c.Dealer)
            .WithMany(u => u.ListedCars)
            .HasForeignKey(c => c.DealerId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Customer)
            .WithMany(u => u.Rentals)
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Car)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Car>()
    .Property(c => c.Pictures)
    .HasConversion(
        v => string.Join(',', v),
        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
    )
    .Metadata.SetValueComparer(new ValueComparer<List<string>>(
        (c1, c2) => c1.SequenceEqual(c2),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c.ToList()
    ));
    }
}
