using AutoMapper;
using Core.Entities;
using Core.enums;
using Core.Interfaces;
using Core.Models;
using APIPart.DTOs.CarDtos;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using APIPart.DTOs;
using APIPart.ErrorHandling;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController : Controller
    {
        private IGenericRepository<Car> _carRepository;

        private readonly IMapper _mapper;
        private IGenericRepository<Driver> _driverRepository;
        public CarController(IGenericRepository<Car> carRepository, IMapper mapper, IGenericRepository<Driver> driverRepository)
        {
            _carRepository = carRepository;
            _mapper = mapper;
            _driverRepository = driverRepository;
        }

        [Route("GetCars")]

        [HttpGet]
        public async Task<ApiResponse> GetCars([FromQuery] ListRequestDto listRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var searchWord = listRequestDto.SearchWord.ToLower();
            /*
            var query = _carRepository.GetQueryable()
                .Where(c =>
                    c.Number.ToLower().Contains(searchWord) ||
                    c.Type.ToLower().Contains(searchWord) ||
                    c.Color.ToLower().Contains(searchWord) ||
                    c.DailyFare.ToString().Contains(searchWord) ||
                    c.HasDriver.ToString().ToLower().Contains(searchWord) ||
                    c.IsAvailable.ToString().ToLower().Contains(searchWord) ||
                    (c.DriverId != null && c.DriverId.ToString().Contains(searchWord))
                );*/
          var query = _carRepository.GetQueryable()
          .GroupJoin(
              _driverRepository.GetQueryable(),
              car => car.DriverId,
              driver => driver.Id,
              (car, drivers) => new { Car = car, Drivers = drivers.DefaultIfEmpty() }
          )
          .SelectMany(
              x => x.Drivers,
              (carResult, driver) => new { Car = carResult.Car, Driver = driver }
          )
          .Where(c =>
              c.Car.Number.ToLower().Contains(searchWord) ||
              c.Car.Type.ToLower().Contains(searchWord) ||
              c.Car.Color.ToLower().Contains(searchWord) ||
              c.Car.DailyFare.ToString().Contains(searchWord) ||
              c.Car.HasDriver.ToString().ToLower().Contains(searchWord) ||
              c.Car.IsAvailable.ToString().ToLower().Contains(searchWord) ||
              (c.Driver != null && c.Driver.Name.ToLower().Contains(searchWord))
          )
          .Select(c => c.Car);
          
            if (query == null)
            {
                return new ApiResponse(404, "No Cars ");
            }
            var count = await  query.CountAsync();
            if (listRequestDto.SortingType == SortingType.asc)
            {
                query = query.OrderBy(c => c.Number);
            }
            else if (listRequestDto.SortingType == SortingType.desc)
            {
                query = query.OrderByDescending(c => c.Number);
            }
            var pageIndex = listRequestDto.PageNumber - 1;
            var pageSize = listRequestDto.PageSize;

            query = query.Skip(pageIndex * pageSize).Take(pageSize);
            var cars = await query.ToListAsync();

            var carPaginationDto = _mapper.Map<CarPaginationDto>(cars);
            carPaginationDto.Count = count;
            return new ApiOkResponse(carPaginationDto); ;
        }
        [HttpGet]
        public async Task<ApiResponse> GetList()
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var cars = await _carRepository.
                        GetAllAsync();
            if (cars == null)
            {
                return new ApiResponse(404, "No cars found");
            }
            IEnumerable<CarListDto>? carListDto = _mapper.Map<IEnumerable<CarListDto>>(cars);
            return new ApiOkResponse(carListDto);
        }
        [HttpGet("{id}")]
        public async Task<ApiResponse> Get(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }

            var car = await _carRepository.GetByIdAsync(id);

            if (car == null)
            {
                return new ApiResponse(404, "Car not found with id " + id.ToString());
            }
            CarDTO carDto = _mapper.Map<CarDTO>(car);

            return new ApiOkResponse(carDto);



        }
        [HttpPost]
        public async Task<ApiResponse> Create(CreateCarDto createCarDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            bool HasDriver = createCarDto.DriverId.HasValue;
            if (HasDriver)
            {
               
                var driver = await _driverRepository.GetByIdAsync(createCarDto.DriverId.Value);
                if (driver == null)
                {
                    return new ApiResponse(400, "Invalid DriverId specified, no driver have this id");
                }
            }
            Car toCreateCar = _mapper.Map<Car>(createCarDto);
            toCreateCar.HasDriver = HasDriver;
            try {var createdCar = await _carRepository.AddAsync(toCreateCar);
                var createdCarDto = _mapper.Map<CarDTO>(createdCar);
                return new ApiOkResponse(createdCarDto);
            }

            catch (Exception ex)
            {

                return new ApiResponse(400, ex.Message);
            }
       
           

        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> Update(Guid id, UpdateCarDto updateCarDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                return new ApiResponse(404, "Car not found with id " + id.ToString());
            }

            var newCar = _mapper.Map<Car>(updateCarDto);

            try { await _carRepository.UpdateAsync(id, newCar); }

            catch (Exception ex)
            {

                return new ApiResponse(400, ex.Message);
            }
            return new ApiOkResponse(updateCarDto);
        }
    
        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var car = await _carRepository.GetByIdAsync(id);
            if (car == null)
            {
                return new ApiResponse(404, "Car not found with id " + id.ToString());
            }
            await _carRepository.DeleteAsync(id);

            CarDTO carDto = _mapper.Map<CarDTO>(car);

            return new ApiOkResponse(carDto);
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
             /*
            var car = _carRepository.GetById(id);
            if (car == null)
            {
                return null;
            }
            car.Number = updateCarDto.Number;
            car.Type = updateCarDto.Type;
            car.EngineCapacity = updateCarDto.EngineCapacity;
            car.Color = updateCarDto.Color;
            car.DailyFare = updateCarDto.DailyFare;

            car.DriverId = updateCarDto.DriverId;
            _carRepository.Update(id,car);*/
            /*
            var car = new Car
            {
                Number = createCarDto.Number,
                Type = createCarDto.Type,
                EngineCapacity = createCarDto.EngineCapacity,
                Color = createCarDto.Color,
                DailyFare = createCarDto.DailyFare,

                //DriverId = createCarDto.DriverId
            };
            _carRepository.Add(car);
            CreateCarDto carDto = _mapper.Map<CreateCarDto>(car);
            return carDto;
            */

        }
}



