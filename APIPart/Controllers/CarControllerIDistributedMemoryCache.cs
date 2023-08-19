using APIPart.DTOs.CarDtos;
using APIPart.ErrorHandling;
using AutoMapper;
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
            var columnName = carRequestDto.SortingColumn;
            switch (columnName)
            {
                case 0:
                    query = carRequestDto.SortingType == SortingType.asc
                        ? query.OrderBy(c => c.Number)
                        : query.OrderByDescending(c => c.Number);
                    break;
                case (CarSortingColumn)1:
                    query = carRequestDto.SortingType == SortingType.asc
                        ? query.OrderBy(c => c.Type)
                        : query.OrderByDescending(c => c.Type);
                    break;
                case (CarSortingColumn)2:
                    query = carRequestDto.SortingType == SortingType.asc
                ? query.OrderBy(c => c.EngineCapacity)
                : query.OrderByDescending(c => c.EngineCapacity);
                    break;
                case (CarSortingColumn)3:
                    query = carRequestDto.SortingType == SortingType.asc
                ? query.OrderBy(c => c.Color)
                : query.OrderByDescending(c => c.Color);
                    break;
                case (CarSortingColumn)4:
                    query = carRequestDto.SortingType == SortingType.asc
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


    }
}
