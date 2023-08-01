using API.DTOs;
using Core.Entities;
using Core.Interfaces;
using infrastracture.Services;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController 
    {
        private readonly CarService _service;

        public CarController(CarService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var cars = _service.
                GetAllCars().Select(c => new CarDTO
            {
                Id = c.Id,
                CarNumber = c.CarNumber,
                Type = c.Type,
                EngineCapacity = c.EngineCapacity,
                Color = c.Color,
                DailyFare = c.DailyFare,
               
                DriverId = c.DriverId
            });
            return Ok(cars);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var car = _service.GetCarById(id);
            if (car == null)
            {
                return NotFound();
            }
            var carDTO = new CarDTO
            {
                Id = car.Id,
                CarNumber = car.CarNumber,
                Type = car.Type,
                EngineCapacity = car.EngineCapacity,
                Color = car.Color,
                DailyFare = car.DailyFare,
               
                DriverId = car.DriverId
            };
            return Ok(carDTO);
        }

        [HttpPost]
        public IActionResult Create(CarDTO carDTO)
        {
            var car = new Car
            {
                CarNumber = carDTO.CarNumber,
                Type = carDTO.Type,
                EngineCapacity = carDTO.EngineCapacity,
                Color = carDTO.Color,
                DailyFare = carDTO.DailyFare,
               
                DriverId = carDTO.DriverId
            };
            _service.AddCar(car);
            return CreatedAtAction(nameof(GetById), new { id = car.Id }, car);
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, CarDTO carDTO)
        {
            var car = _service.GetCarById(id);
            if (car == null)
            {
                return NotFound();
            }
            car.CarNumber = carDTO.CarNumber;
            car.Type = carDTO.Type;
            car.EngineCapacity = carDTO.EngineCapacity;
            car.Color = carDTO.Color;
            car.DailyFare = carDTO.DailyFare;
        
            car.DriverId = carDTO.DriverId;
            _service.UpdateCar(car);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var car = _service.GetCarById(id);
            if (car == null)
            {
                return NotFound();
            }
            _service.DeleteCar(car);
            return NoContent();
        }
        public IActionResult Index()
        {
            var cars = _service.GetAllCars().Select(c => new CarDTO
            {
                Id = c.Id,
                CarNumber = c.CarNumber,
                Type = c.Type,
                EngineCapacity = c.EngineCapacity,
                Color = c.Color,
                DailyFare = c.DailyFare,
                DriverId = c.DriverId
            }).ToList();
           /* var viewModel = new CarIndexViewModel
            {
                Cars = cars
            };
            return View(viewModel);*/
        }

       
    }
}


