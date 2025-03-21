using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using MoRent_V2.Context;
using MoRent_V2.Models;
using MoRent_V2.Models.Dto;

namespace MoRent_V2.Services
{
    public class CarServices(ApplicationDbContext context, CloudinaryService cloudinaryService, IMapper mapper)
    {
        private readonly ApplicationDbContext _context = context;
        private readonly CloudinaryService _cloudinaryService = cloudinaryService;
        private readonly IMapper _mapper = mapper;

        public async Task<Car> AddCarAsync(Car car, List<IFormFile> images)
        {
            if (car.AvailableForRent + car.AvailableForSale > car.TotalQuantity)
            {
                throw new ArgumentException("Your Maths are no mathing.");
            }
            if (images == null || images.Count == 0)
            {
                throw new ArgumentException("At least one image is required.");
            }

            car.Pictures = await _cloudinaryService.UploadImagesAsync(images);

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return car;
        }

        public async Task<Car> UpdateCarAsync(int carId, CarDto carUpdateDto, List<IFormFile>? newImages = null)
        {
            var car = await _context.Cars.FindAsync(carId) ?? throw new ArgumentException("Car not found.");


            _mapper.Map(carUpdateDto, car);

            if (newImages != null && newImages.Count > 0)
            {

                var uploadedImageUrls = await _cloudinaryService.UploadImagesAsync(newImages);


                car.Pictures = uploadedImageUrls;
            }

            _context.Cars.Update(car);
            await _context.SaveChangesAsync();

            return car;
        }
        public async Task<Car> PatchCarAsync(int carId, JsonPatchDocument<Car> patchDocument, List<IFormFile>? newImages = null)
        {
            var car = await _context.Cars.FindAsync(carId) ?? throw new ArgumentException("Car not found.");

            // Apply the patch document to the car
            patchDocument.ApplyTo(car);

            // Handle Cloudinary logic if new images are provided
            if (newImages != null && newImages.Count > 0)
            {
                // Upload new images to Cloudinary
                var uploadedImageUrls = await _cloudinaryService.UploadImagesAsync(newImages);

                // Append the new images to the existing Pictures list
                car.Pictures.AddRange(uploadedImageUrls);
            }

            await _context.SaveChangesAsync();

            return car;
        }
        public async Task<Car> UpdateCarAvailabilityAsync(int carId, int additionalQuantity, int additionalForSale, int additionalForRent)
        {

            var car = await _context.Cars.FindAsync(carId) ?? throw new ArgumentException("Car not found.");
            if (additionalForSale + additionalForRent > additionalQuantity)
            {
                throw new ArgumentException("The sum of additional cars for sale and rent cannot exceed the additional quantity.");
            }

            car.TotalQuantity += additionalQuantity;
            car.AvailableForSale += additionalForSale;
            car.AvailableForRent += additionalForRent;

            if (additionalForSale > 0)
            {
                car.IsForSale = true;
            }
            if (additionalForRent > 0)
            {
                car.IsForRent = true;
            }

            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
            return car;
        }

        public async Task BuyCarAsync(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null || car.AvailableForSale <= 0)
            {
                throw new InvalidOperationException("Car not available for sale.");
            }

            car.AvailableForSale--;
            car.TotalQuantity--;

            if (car.AvailableForSale == 0)
            {
                car.IsForSale = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RentCarAsync(int carId)
        {
            var car = await _context.Cars.FindAsync(carId);
            if (car == null || car.AvailableForRent <= 0)
            {
                throw new InvalidOperationException("Car not available for rent.");
            }

            car.AvailableForRent--;
            car.TotalQuantity--;

            if (car.AvailableForRent == 0)
            {
                car.IsForRent = false;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Car> GetCarByIdAsync(int carId)
        {
            var car = await _context.Cars
                .Include(c => c.Dealer)
                .FirstOrDefaultAsync(c => c.Id == carId) ?? throw new ArgumentException("Car not found.");
            return car;
        }
        public async Task<List<CarDto>> GetAllCarsAsync()
        {

            var cars = await _context.Cars
                .Include(c => c.Dealer)
                .ToListAsync();

            var carDtos = _mapper.Map<List<CarDto>>(cars);

            return carDtos;
        }


    }
}