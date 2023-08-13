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

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriverControllerUOW : Controller
    {
        private readonly IMapper _mapper;
        private IDriverService _driverService;

        public DriverControllerUOW(ICarService carService, IMapper mapper, IDriverService driverService)
        {
            _mapper = mapper;
            _driverService = driverService;
        }
        [Route("GetDrivers")]

        [HttpGet]
        public async Task<ApiResponse> GetDriversAsync([FromQuery] ListRequestDto listRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var searchWord = listRequestDto.SearchWord.ToLower();

            var query = _driverService.GetQueryable()
                .Where(c =>
                    c.Name.ToLower().Contains(searchWord) ||
                    c.Phone.ToLower().Contains(searchWord) ||
                    c.LicenseNumber.ToLower().Contains(searchWord)
                );
            var count = await query.CountAsync();
            if (listRequestDto.SortingType == SortingType.asc)
            {
                query = query.OrderBy(c => c.Name);
            }
            else if (listRequestDto.SortingType == SortingType.desc)
            {
                query = query.OrderByDescending(c => c.Name);
            }
            var pageIndex = listRequestDto.PageNumber - 1;
            var pageSize = listRequestDto.PageSize;

            query = query.Skip(pageIndex * pageSize).Take(pageSize);
            var drivers = await query.ToListAsync();

            var driverPaginationDto = _mapper.Map<DriverPaginationDto>(drivers);
            driverPaginationDto.Count = count;
            return new ApiOkResponse(driverPaginationDto);
        }
        [HttpGet]
        public async Task<ApiResponse> GetListAsync()
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var drivers = await _driverService.GetAllAsync();


            IEnumerable<DriverListDto>? driverListDto = _mapper.Map<IEnumerable<DriverListDto>>(drivers);
            return new ApiOkResponse(driverListDto);
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


            toCreateDriver.HasReplacement = HasReplacement;
            try
            {
                var createdDriver = await _driverService.AddAsync(toCreateDriver);
                var createdDriverDto = _mapper.Map<CarDTO>(createdDriver);
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
            newDriver.HasReplacement = HasReplacement;
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
            await _driverService.DeleteAsync(id);

            DriverDto driverDto = _mapper.Map<DriverDto>(driver);
            return new ApiOkResponse("driver with id" + id + "is deleted");
        }
    }
}
