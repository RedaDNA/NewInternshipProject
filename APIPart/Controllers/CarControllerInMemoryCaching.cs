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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public  ApiResponse  GetCars([FromQuery] CarRequestDto carRequestDto)
        {

            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            if ( _memoryCache.TryGetValue("CarList", out List<Car> cachedCarList))
            {
                var searchWord =  carRequestDto.SearchWord.ToLower();
                var query =  cachedCarList.Where(c =>
                c.Number.ToLower().Contains(searchWord) ||
                c.Type.ToLower().Contains(searchWord) ||
                c.Color.ToLower().Contains(searchWord) ||
                c.DailyFare.ToString().Contains(searchWord) ||
                (c.Driver != null && c.Driver.Name.ToLower().Contains(searchWord)));
                var count =  query.Count();
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
                


              var  carPaginationDto = _mapper.Map<CarPaginationDto>(query);
                carPaginationDto.Count = count;
                return new ApiOkResponse(carPaginationDto);
            }
            return new ApiResponse(
                404
              ,"Cached data not found.");
          
        }
       
        [HttpGet("{id}")]
        public  ApiResponse Get(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            if (_memoryCache.TryGetValue("CarList", out List<Car> cachedCarTable))
            {

                var car =  cachedCarTable.FirstOrDefault(c => c.Id == id);

                if (car == null)
                {
                    return new ApiResponse(404, "Car not found with id " + id.ToString());
                }
                CarDTO carDto = _mapper.Map<CarDTO>(car);

                return new ApiOkResponse(carDto);
            }
            return new ApiResponse(404, "Cached data not found.");
        
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
    
            try
            {
                var createdCar = await _carService.AddAsync(toCreateCar);
                var createdCarDto = _mapper.Map<CarDTO>(createdCar);
                var createdCarListDto = _mapper.Map<CarListDto>(createdCar);

                if (_memoryCache.TryGetValue("CarList", out List<Car> cachedCarList))
                {
                    cachedCarList.Add(createdCar); 
                    _memoryCache.Set("CarList", cachedCarList);
                }

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
            
            newCar.Id = id;
            try
            {
                await _carService.UpdateAsync(id, newCar);

             
            
                if (_memoryCache.TryGetValue("CarList", out List<Car> CarList))
                {
                    var updatedCar = CarList.FirstOrDefault(c => c.Id == id);
                    if (updatedCar != null)
                    {
                      
                        CarList.Remove(updatedCar);
                        CarList.Add(newCar);
                        _memoryCache.Set("CarList", CarList);
                    }
                }

                return new ApiOkResponse(updateCarDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(400, ex.Message);
            }
           
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
            await _carService.DeleteAsync(id);
           
            
            if (_memoryCache.TryGetValue("CarList", out List<Car> CarList))
            {
                var deletedCar = CarList.FirstOrDefault(c => c.Id == id);
                if (deletedCar != null)
                {
                    CarList.Remove(deletedCar);
                    _memoryCache.Set("CarList", CarList);
                }
            }
            return new ApiOkResponse("car with id" + id + "is deleted");
        }



    }



}

