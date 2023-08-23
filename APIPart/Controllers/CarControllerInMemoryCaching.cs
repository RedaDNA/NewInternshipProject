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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Text.Json;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarControllerInMemoryCaching : Controller
    {
        public readonly ICarService _carService;
        public readonly IRentalService _rentalService;
        public readonly IMemoryCache _memoryCache;

        private readonly IMapper _mapper;
        private IDriverService _driverService;

        public CarControllerInMemoryCaching(ICarService carService, IMapper mapper, IDriverService driverService, IRentalService rentalService
            ,IMemoryCache memoryCache)
        {
            _carService = carService;
            _mapper = mapper;
            _rentalService = rentalService;
            _driverService = driverService;
            _memoryCache=memoryCache;
        }
        [Route("GetCars")]

        [HttpGet]
        public async Task<ApiResponse> GetCarsAsync([FromQuery] CarRequestDto carRequestDto)
        {
            var cacheKey = GetCacheKey(carRequestDto);
            if (_memoryCache.TryGetValue(cacheKey, out CarPaginationDto cachedCarPaginationDto))
            {
                return new ApiOkResponse(cachedCarPaginationDto);
            }

            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var searchWord = carRequestDto.SearchWord.ToLower();
       

            if (!_memoryCache.TryGetValue(searchWord, out CarPaginationDto carPaginationDto)) {
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
            

            carPaginationDto = _mapper.Map<CarPaginationDto>(cars);
            carPaginationDto.Count = count;
                var cachExpirationOption = new MemoryCacheEntryOptions {
                    AbsoluteExpiration = DateTime.Now.AddHours(1), SlidingExpiration = TimeSpan.FromMinutes(1)
            };

                _memoryCache.Set(cacheKey, carPaginationDto,cachExpirationOption); 
            }
            return new ApiOkResponse(carPaginationDto);

        }
        private string GetCacheKey(CarRequestDto carRequestDto)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var serializedDto = System.Text.Json.JsonSerializer.Serialize(carRequestDto, options);
            return $"CarRequest:{serializedDto}";
        }
    }



}

