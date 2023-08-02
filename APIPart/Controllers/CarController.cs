using APIPart.DTOs;
using Core.Entities;
using Core.Interfaces;

using infrastructure.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController : Controller
    {
        private IGenericRepository<Car> _carRepository;


        public CarController(IGenericRepository<Car> carRepository) {
            _carRepository = carRepository;

        }

        [HttpGet]
        public IActionResult GetAll()
        { 
        return Ok(_carRepository.GetAll());
        
        }
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            return Ok(_carRepository.GetById(id));
        }
        [HttpPost]
        public IActionResult Create(Car car)
        {

            _carRepository.Add(car);

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Car car)
        {

            _carRepository.Update(car);

            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(Car car)
        {
            _carRepository.Delete(car);

            return Ok();
        }



            /*
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
                        Number = c.Number,
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
                    Number = car.Number,
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
                    Number = carDTO.Number,
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
                car.Number = carDTO.Number;
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
                    Number = c.Number,
                    Type = c.Type,
                    EngineCapacity = c.EngineCapacity,
                    Color = c.Color,
                    DailyFare = c.DailyFare,
                    DriverId = c.DriverId
                }).ToList();

                return NoContent();
            }
            */

        }
}



