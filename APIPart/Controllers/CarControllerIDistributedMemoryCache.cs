using APIPart.DTOs.CarDtos;
using APIPart.ErrorHandling;
using AutoMapper;
using Core.Entities;
using Core.enums;
using Core.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarControllerIDistributedMemoryCache : Controller
    {
        public readonly ICarService _carService;
        public readonly IRentalService _rentalService;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;
        private IDriverService _driverService;

        public CarControllerIDistributedMemoryCache(ICarService carService, IMapper mapper, IDriverService driverService, IRentalService rentalService
            , IDistributedCache cach)
        {
            _carService = carService;
            _mapper = mapper;
            _rentalService = rentalService;
            _driverService = driverService;
            _cache = cach;
        }
        [Route("GetCars")]

        [HttpGet]
        public async Task<ApiResponse> GetCarsAsync([FromQuery] CarRequestDto carRequestDto)
        {
            var cacheKey = $"GetCars-{carRequestDto.SearchWord}-{carRequestDto.SortingColumn}-{carRequestDto.SortingType}-{carRequestDto.PageNumber}-{carRequestDto.PageSize}";
            var cachedResult = await _cache.GetStringAsync(cacheKey);
            if (cachedResult != null)
            {
            
                var cachedCarPaginationDto = JsonConvert.DeserializeObject<CarPaginationDto>(cachedResult);
                return new ApiOkResponse(cachedCarPaginationDto);
            }
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }

            var searchWord = carRequestDto.SearchWord.ToLower();


            IQueryable<Car> query = _carService.GetQueryable().Include(r => r.Driver);
            if (!string.IsNullOrEmpty(carRequestDto.SearchWord))
            {
                query = query.Where(c =>
        c.Number.ToLower().Contains(searchWord) ||
        c.Type.ToLower().Contains(searchWord) ||
        c.Color.ToLower().Contains(searchWord) ||
        c.DailyFare.ToString().Contains(searchWord) ||
        (c.Driver != null && c.Driver.Name.ToLower().Contains(searchWord))
    );

            }


            var count = await query.CountAsync();
            var columnName = carRequestDto.SortingColumn.ToLower();
            switch (columnName)
            {
                case "number":
                    query = carRequestDto.SortingType == "asc"
                        ? query.OrderBy(c => c.Number)
                        : query.OrderByDescending(c => c.Number);
                    break;
                case ("type"):
                    query = carRequestDto.SortingType == "asc"
                        ? query.OrderBy(c => c.Type)
                        : query.OrderByDescending(c => c.Type);
                    break;
                case ("enginecapacity"):
                    query = carRequestDto.SortingType == "asc"
                ? query.OrderBy(c => c.EngineCapacity)
                : query.OrderByDescending(c => c.EngineCapacity);
                    break;
                case ("color"):
                    query = carRequestDto.SortingType == "asc"
                        ? query.OrderBy(c => c.Color)
                : query.OrderByDescending(c => c.Color);
                    break;
                case ("dailyfare"):
                    query = carRequestDto.SortingType == "asc"
                ? query.OrderBy(c => c.DailyFare)
                : query.OrderByDescending(c => c.DailyFare);
                    break;


                default:

                    query = query.OrderBy(c => c.Number);
                    break;
            }
            var pageIndex = carRequestDto.PageNumber - 1;
            var pageSize = carRequestDto.PageSize;

            query = query.Skip(pageIndex * pageSize).Take(pageSize);
            var cars = await query.ToListAsync();

            var carPaginationDto = _mapper.Map<CarPaginationDto>(cars);
            carPaginationDto.Count = count;
            var jsonResult = JsonConvert.SerializeObject(carPaginationDto);
            var cacheOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Set the desired cache expiration time
            await _cache.SetStringAsync(cacheKey, jsonResult, cacheOptions);

            return new ApiOkResponse(carPaginationDto); ;
        }
        [HttpGet]
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
            //     toCreateCar.HasDriver = HasDriver;
            try
            {
                var createdCar = await _carService.AddAsync(toCreateCar);
                var createdCarDto = _mapper.Map<CarDTO>(createdCar);
                await _cache.RemoveAsync("GetCars-*");
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
            // newCar.HasDriver = HasDriver;
            newCar.Id = id;
            try { await _carService.UpdateAsync(id, newCar);
                await _cache.RemoveAsync("GetCars-*");
            }

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
            var carUsedInRental = await _rentalService.IsCarExistInAsync(id);
            if (carUsedInRental)
            {
                return new ApiResponse(404, "Cannot delete the car, Car is already used in rental record");


            }
            try
            {
                await _carService.DeleteAsync(id);

                // Invalidate cache for GetCarsAsync API
                await _cache.RemoveAsync("GetCars-*");

                return new ApiOkResponse("Car with id " + id + " is deleted");
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
        }



    }
}
