using APIPart.DTOs.CarDtos;
using APIPart.DTOs.DriverDtos;
using APIPart.DTOs;
using APIPart.ErrorHandling;
using AutoMapper;
using Core.enums;
using Core.Interfaces;
using Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using infrastructure.Services;
using infrastructure.Migrations;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriverController : Controller
    {
        private readonly IMapper _mapper;
        private IDriverService _driverService;
        private IRentalService _rentalService;

        public DriverController(ICarService carService, IMapper mapper, IDriverService driverService, IRentalService rentalService)
        {
            _mapper = mapper;
            _driverService = driverService;
            _rentalService = rentalService;
        }

        [HttpGet]
        public async Task<ApiResponse> GetDriversAsync([FromQuery] DriverRequestDto driverRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }

            var query = _driverService.GetQueryable();
            if (!string.IsNullOrEmpty(driverRequestDto.SearchWord))
            {
                var searchWord = driverRequestDto.SearchWord.ToLower();

                query = query
                    .Where(c =>
                        c.Name.ToLower().Contains(searchWord) ||
                        c.Phone.ToLower().Contains(searchWord) ||
                        c.LicenseNumber.ToLower().Contains(searchWord)
                    );
            }
            var columnName = driverRequestDto.SortingColumn.ToLower();
            var count = await query.CountAsync();
            switch (columnName)
            {
                case "name":
                    query = driverRequestDto.SortingType == "asc"
                        ? query.OrderBy(c => c.Name)
                        : query.OrderByDescending(c => c.Name);
                    break;
                case ("licensenumber"):
                    query = driverRequestDto.SortingType == "asc"
                        ? query.OrderBy(c => c.LicenseNumber)
                        : query.OrderByDescending(c => c.LicenseNumber);
                    break;
                case ("phone"):
                    query = driverRequestDto.SortingType == "asc"
                ? query.OrderBy(c => c.Phone)
                : query.OrderByDescending(c => c.Phone);
                    break;       
                default:

                    query = query.OrderBy(c => c.Name);
                    break;
            }
          
           
            var pageIndex = driverRequestDto.PageNumber - 1;
            var pageSize = driverRequestDto.PageSize;

            query = query.Skip(pageIndex * pageSize).Take(pageSize);
            var drivers = await query.ToListAsync();

            var driverPaginationDto = _mapper.Map<DriverPaginationDto>(drivers);
            driverPaginationDto.Count = count;
            return new ApiOkResponse(driverPaginationDto);
        }
      
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var driver = await _driverService.GetByIdAsync(id);
            if (driver == null)
            {
                return new ApiResponse(404, "driver not found with id " + id.ToString());
            }
            DriverDto driverDto = _mapper.Map<DriverDto>(driver);
            return new ApiOkResponse(driverDto);
        }
        [HttpPost]
        public async Task<ApiResponse> CreateAsync(CreateDriverDto createDriverDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            bool HasReplacement = createDriverDto.ReplacementDriverId.HasValue;
            if (HasReplacement)
            {

                var replacementDriver = await _driverService.IsExistAsync(createDriverDto.ReplacementDriverId.Value);
                if (!replacementDriver)
                {
                    return new ApiResponse(400, "Invalid DriverId specified, no driver have this id");
                }
            }


            Driver toCreateDriver = _mapper.Map<Driver>(createDriverDto);


          //  toCreateDriver.HasReplacement = HasReplacement;
            try
            {
                var createdDriver = await _driverService.AddAsync(toCreateDriver);
                var createdDriverDto = _mapper.Map<DriverDto>(createdDriver);
                return new ApiOkResponse(createdDriverDto);
            }

            catch (Exception ex)
            {

                return new ApiResponse(400, ex.Message);
            }



        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> UpdateAsync(Guid id, UpdateDriverDto updateDriverDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }

            var driver = await _driverService.GetByIdAsync(id);
            if (driver == null)
            {
                return new ApiResponse(404, "Driver not found with id " + id.ToString());
            }
            bool HasReplacement = updateDriverDto.ReplacementDriverId.HasValue;
            if (HasReplacement)
            {

                var replacementDriver = await _driverService.IsExistAsync(updateDriverDto.ReplacementDriverId.Value);
                if (!replacementDriver)  
                {
                    return new ApiResponse(400, "Invalid replacement Driver id specified, no driver have this id");
                }
            }
            var newDriver = _mapper.Map<Driver>(updateDriverDto);
           // newDriver.HasReplacement = HasReplacement;
            newDriver.Id = id;
            try { await _driverService.UpdateAsync(id, newDriver); }

            catch (Exception ex)
            {

                return new ApiResponse(400, ex.Message);
            }

            return new ApiOkResponse(newDriver);

        }
        [HttpDelete("{id}")]
        public async Task<ApiResponse> DeleteAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }

            var driver = await _driverService.IsExistAsync(id);
            if (!driver)
            {
                return new ApiResponse(404, "driver not found with id " + id.ToString());
            }
            var driverUsedInRental = await _rentalService.IsDriverExistInAsync(id);
            if (driverUsedInRental)
            {
                return new ApiResponse(404, "Cannot delete the driver, Driver is already used in rental record");


            }
            await _driverService.DeleteAsync(id);

            DriverDto driverDto = _mapper.Map<DriverDto>(driver);
            return new ApiOkResponse("driver with id" + id + "is deleted");
        }




     


    

    }
}
