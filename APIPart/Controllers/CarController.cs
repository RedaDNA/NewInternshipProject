using APIPart.DTOs;
using APIPart.DTOs.CarDtos;
using APIPart.ErrorHandling;
using AutoMapper;
using Core.Entities;
using Core.enums;
using Core.Interfaces;
using Core.Interfaces.IServices;
using infrastructure.Services;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController : Controller
    {
        public readonly ICarService _carService;
        private readonly IMapper _mapper;
        private IDriverService _driverService;

        public CarController(ICarService carService, IMapper mapper, IDriverService driverService)
        {
            _carService = carService;
            _mapper = mapper;
            _driverService = driverService;
        }
        [Route("GetCars")]

        [HttpGet]
        public async Task<ApiResponse> GetCarsAsync([FromQuery] ListRequestDto listRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var searchWord = listRequestDto.SearchWord.ToLower();

            var query = _carService.GetQueryable()
            .GroupJoin(
                _driverService.GetQueryable(),
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


            var count = await query.CountAsync();
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
        public async Task<IActionResult> GetCarList()
        {
            var carDetailsList = await _carService.GetAllAsync();
            if (carDetailsList == null)
            {
                return NotFound();
            }
            return Ok(carDetailsList);
        }


        [HttpGet("{id}")]
        public async Task<ApiResponse> GetAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }

            var car = await _carService.GetByIdAsync(id);

            if (car == null)
            {
                return new ApiResponse(404, "Car not found with id " + id.ToString());
            }
            CarDTO carDto = _mapper.Map<CarDTO>(car);

            return new ApiOkResponse(carDto);



        }

        [HttpPost]
        public async Task<ApiResponse> CreateAsync(CreateCarDto createCarDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            bool HasDriver = createCarDto.DriverId.HasValue;
            if (HasDriver)
            {

                var driver = await _driverService.IsExistAsync(createCarDto.DriverId.Value);
                if (!driver)
                {
                    return new ApiResponse(400, "Invalid DriverId specified, no driver have this id");
                }
            }
            Car toCreateCar = _mapper.Map<Car>(createCarDto);
            toCreateCar.HasDriver = HasDriver;
            try
            {
                var createdCar = await _carService.AddAsync(toCreateCar);
                var createdCarDto = _mapper.Map<CarDTO>(createdCar);
                return new ApiOkResponse(createdCarDto);
            }

            catch (Exception ex)
            {

                return new ApiResponse(400, ex.Message);
            }



        }

      
        [HttpPut("{id}")]
        public async Task<ApiResponse> UpdateAsync(Guid id, UpdateCarDto updateCarDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var car = await _carService.GetByIdAsync(id);
            if (car == null)
            {
                return new ApiResponse(404, "Car not found with id " + id.ToString());
            }
            bool HasDriver = updateCarDto.DriverId.HasValue;
            if (HasDriver)
            {

                var driver = await _driverService.IsExistAsync(updateCarDto.DriverId.Value);
                if (!driver)
                {
                    return new ApiResponse(400, "Invalid DriverId specified, no driver have this id");
                }
            }
            var newCar = _mapper.Map<Car>(updateCarDto);
            newCar.HasDriver = HasDriver;
            newCar.Id = id;
            try { await _carService.UpdateAsync(id, newCar); }

            catch (Exception ex)
            {

                return new ApiResponse(400, ex.Message);
            }
            return new ApiOkResponse(updateCarDto);
        }


        [HttpDelete("{id}")]
        public async Task<ApiResponse> DeleteAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var car = await _carService.IsExistAsync(id);
            if (!car)
            {
                return new ApiResponse(404, "Car not found with id " + id.ToString());
            }
            await _carService.DeleteAsync(id);
            return new ApiOkResponse("car with id" + id + "is deleted");
        }


    }
}

