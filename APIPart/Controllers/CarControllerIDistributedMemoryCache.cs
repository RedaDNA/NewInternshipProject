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


    }
}
