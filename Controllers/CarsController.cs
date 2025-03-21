using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MoRent_Server.Models;
using MoRent_V2.Models;
using MoRent_V2.Models.Dto;
using MoRent_V2.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MoRent_V2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarsController(CarServices carServices, ILogger<CarsController> logger, IMapper mapper) : ControllerBase
{
    private readonly CarServices _carServices = carServices;
    private readonly ILogger<CarsController> _logger = logger;
    private readonly IMapper _mapper = mapper;

    [HttpPost("create")]
    public async Task<IActionResult> CreateCar([FromForm] CarDto carCreateDto, [FromForm] List<IFormFile> images)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var dealerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(dealerId))
        {
            return Unauthorized("Dealer ID not found in claims.");
        }


        var car = _mapper.Map<Car>(carCreateDto);

        car.DealerId = dealerId;

        try
        {
            var addedCar = await _carServices.AddCarAsync(car, images);
            return CreatedAtAction(nameof(GetCar), new { id = addedCar.Id }, addedCar);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCar(int id)
    {
        try
        {
            var car = await _carServices.GetCarByIdAsync(id);
            return Ok(car);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCars()
    {
        try
        {
            var cars = await _carServices.GetAllCarsAsync();
            return Ok(cars);
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while fetching cars.");
        }
    }

    [HttpPut("update/{carId}")]

    public async Task<IActionResult> UpdateCar(int carId, [FromForm] CarDto carUpdateDto, [FromForm] List<IFormFile> images)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var car = await _carServices.UpdateCarAsync(carId, carUpdateDto, images);
            return Ok(car);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPatch("patch/{carId}")]
    public async Task<IActionResult> PatchCar(int carId, [FromBody] JsonPatchDocument<Car> patchDocument, [FromForm] List<IFormFile> images)
    {
        if (patchDocument == null)
        {
            return BadRequest("Patch document is required.");
        }

        try
        {
            var car = await _carServices.PatchCarAsync(carId, patchDocument, images);
            return Ok(car);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("update-availability/{carId}")]
    public async Task<IActionResult> UpdateCarAvailability(int carId, [FromQuery, Required] int additionalQuantity, [FromQuery, Required] int additionalForSale, [FromQuery, Required] int additionalForRent)
    {
        try
        {
            var updatedCar = await _carServices.UpdateCarAvailabilityAsync(carId, additionalQuantity, additionalForSale, additionalForRent);
            return Ok(updatedCar);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("buy/{carId}")]
    public async Task<IActionResult> BuyCar(int carId)
    {
        try
        {
            await _carServices.BuyCarAsync(carId);
            return Ok("You've bought the car.Congratulation!!!!.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("rent/{carId}")]
    public async Task<IActionResult> RentCar(int carId)
    {
        try
        {
            await _carServices.RentCarAsync(carId);
            return Ok("The car has been rented.");
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}