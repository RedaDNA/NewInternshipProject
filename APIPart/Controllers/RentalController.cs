using APIPart.DTOs;
using APIPart.DTOs.RentalDtos;
using APIPart.ErrorHandling;
using AutoMapper;
using Core.Entities;
using Core.enums;
using Core.Interfaces;
using Core.Interfaces.IServices;
using infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RentalController : Controller
    {
        public  ICarService _carService;
        private readonly IMapper _mapper;
        private IDriverService _driverService;
        private IRentalService _rentalService;
        private ICustomerService _customerService;

        public RentalController(ICarService carService, IMapper mapper,
          IDriverService driverService,
       IRentalService rentalService,
           ICustomerService customerService


            )
        {
            _rentalService = rentalService;
            _carService = carService;
            _driverService = driverService;
            _customerService = customerService;
            _mapper = mapper;
        }
        [Route("GetRentals")]

        [HttpGet]
        public async Task<ApiResponse> GetRentalsAsync([FromQuery] ListRequestDto listRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var searchWord = listRequestDto.SearchWord.ToLower();

            var query = _rentalService.GetQueryable();
                
            var count =await query.CountAsync();
            if (listRequestDto.SortingType == SortingType.asc)
            {
                query = query.OrderBy(c => c.StartDate);
            }
            else if (listRequestDto.SortingType == SortingType.desc)
            {
                query = query.OrderByDescending(c => c.StartDate);
            }
            var pageIndex = listRequestDto.PageNumber - 1;
            var pageSize = listRequestDto.PageSize;

            query = query.Skip(pageIndex * pageSize).Take(pageSize);
            var drivers = await query.ToListAsync();

            var rentalPaginationDto = _mapper.Map<RentalPaginationDto>(drivers);
            rentalPaginationDto.Count = count;
            return new ApiOkResponse(rentalPaginationDto);
        }
        [HttpGet]
        public async Task<ApiResponse> GetListAsync()
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var rentals = await _rentalService.GetAllAsync();
      
         
            IEnumerable<RentalListDto>? driverListDto = _mapper.Map<IEnumerable<RentalListDto>>(rentals);
            return new ApiOkResponse(driverListDto);
        }
        [HttpGet("{id}")]
        public async Task<ApiResponse> GetAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
            var driver = await _rentalService.GetByIdAsync(id);
            if (driver == null)
            {
                return new ApiResponse(404, "rental not found with id " + id.ToString());
            }
            RentalDto driverDto = _mapper.Map<RentalDto>(driver);
            return new ApiOkResponse(driverDto);
        }
        [HttpPost]
        public async Task<ApiResponse> CreateAsync(CreateRentalDto createRentalDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
                var carExistence = await _carService.IsExistAsync(createRentalDto.CarId);
                if (!carExistence)
                {
                    return new ApiResponse(400, "Invalid car id specified, no car have this id");
                }
            var driverExistence = await _driverService.IsExistAsync(createRentalDto.DriverId.Value);
            if (!driverExistence)
            {
                return new ApiResponse(400, "Invalid driver id specified, no driver have this id");
            }
            var customerExistence = await _customerService.IsExistAsync(createRentalDto.CustomerId);
            if (!customerExistence)
            {
                return new ApiResponse(400, "Invalid customer id specified, no customer have this id");
            }

            Rental toCreateRental =  _mapper.Map<Rental>(createRentalDto);
            try
            {
                var createdRental = await   _rentalService.AddAsync(toCreateRental);
                var createdRentalDto = _mapper.Map<RentalDto>(createdRental);
                return new ApiOkResponse(createdRentalDto);
            }

            catch (Exception ex)
            {

                return new ApiResponse(400, ex.Message);
            }


           
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> UpdateAsync(Guid id, UpdateRentalDto updateRentalDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }

            var carExistence = await _carService.IsExistAsync(updateRentalDto.CarId);
            if (!carExistence)
            {
                return new ApiResponse(400, "Invalid car id specified, no car have this id");
            }
            var driverExistence = await _driverService.IsExistAsync(updateRentalDto.DriverId.Value);
            if (!driverExistence)
            {
                return new ApiResponse(400, "Invalid driver id specified, no driver have this id");
            }
            var customerExistence = await _customerService    .IsExistAsync(updateRentalDto.CustomerId);
            if (!customerExistence)
            {
                return new ApiResponse(400, "Invalid customer id specified, no customer have this id");
            }
            
            var newRental = _mapper.Map<Rental>(updateRentalDto);
            newRental.Id = id;
            try { await _rentalService.UpdateAsync(id, newRental); }

            catch (Exception ex)
            {

                return new ApiResponse(400, ex.Message);
            }
         
                return new ApiOkResponse(newRental);

        }
        [HttpDelete("{id}")]
        public async Task<ApiResponse> DeleteAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }
           
            var driver = await _rentalService.IsExistAsync(id);
            if (!driver)
            {
                return new ApiResponse(404, "driver not found with id " + id.ToString());
            }
           await _rentalService.DeleteAsync(id);

            RentalDto driverDto = _mapper.Map<RentalDto>(driver);
            return new ApiOkResponse("driver with id" + id + "is deleted");
        }
    }
}
