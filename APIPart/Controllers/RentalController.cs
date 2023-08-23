using APIPart.DTOs;
using APIPart.DTOs.CarDtos;
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
using Newtonsoft.Json.Linq;
using System.Runtime.ConstrainedExecution;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<ApiResponse> GetRentalsAsync([FromQuery] RentalRequestDto rentalRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return new ApiBadRequestResponse(ModelState);
            }

            IQueryable<Rental> query = _rentalService.GetQueryable().Include(r => r.Car)
       .Include(r => r.Driver)
       .Include(r => r.Customer);


            if (!string.IsNullOrEmpty(rentalRequestDto.SearchWord))
            {
                var searchWord = rentalRequestDto.SearchWord.ToLower();
                query=query.Where(c =>
                c.Car.Number.ToLower().Contains(searchWord) ||
                c.Car.Type.ToLower().Contains(searchWord) ||
                c.Car.Color.ToLower().Contains(searchWord) ||

            c.Customer.Name.ToLower().Contains(searchWord) ||

                (c.Driver != null && c.Driver.Name.ToLower().Contains(searchWord))
            );

            }


            var count =await query.CountAsync();
            var columnName = rentalRequestDto.SortingColumn.ToLower();
            switch (columnName)
            {
                case "startdate":
                    query = rentalRequestDto.SortingType == "asc"
                        ? query.OrderBy(c => c.StartDate)
                        : query.OrderByDescending(c => c.StartDate);
                    break;
                case ("enddate"):
                    query = rentalRequestDto.SortingType == "asc"
                        ? query.OrderBy(c => c.EndDate)
                        : query.OrderByDescending(c => c.EndDate);
                    break;
                case ("status"):
                    query = rentalRequestDto.SortingType == "asc"
                ? query.OrderBy(c => c.Status)
                : query.OrderByDescending(c => c.Status);
                    break;
                case ("customername"):
                    query = rentalRequestDto.SortingType == "asc"
                        ? query.OrderBy(c => c.Customer.Name)
                : query.OrderByDescending(c => c.Customer.Name);
                    break;
                case ("drivername"):
                    query = rentalRequestDto.SortingType == "asc"
                ? query.OrderBy(c => c.Driver.Name)
                : query.OrderByDescending(c => c.Driver.Name);
                    break;
                case ("totalfare"):
                    query = rentalRequestDto.SortingType == "asc"
                ? query.OrderBy(c => c.TotalFare)
                : query.OrderByDescending(c => c.TotalFare);
                    break;
                case ("carnumber"):
                    query = rentalRequestDto.SortingType == "asc"
                ? query.OrderBy(c => c.Car.Number)
                : query.OrderByDescending(c => c.Car.Number);
                    break;
                default:

                    query = query.OrderBy(c => c.StartDate);
                    break;
            }
            var pageIndex = rentalRequestDto.PageNumber - 1;
            var pageSize = rentalRequestDto.PageSize;

            query = query.Skip(pageIndex * pageSize).Take(pageSize);
            var rentals = await query.ToListAsync();
            var rentalListDtos = _mapper.Map<List<RentalListDto>>(rentals);
            var rentalPaginationDto = new RentalPaginationDto
            {
                Count = count,
                RentalList = rentalListDtos
            };

           /* var rentalPaginationDto = new RentalPaginationDto
            {
                Count = count,
                RentalList = rentals.Select(r => new RentalListDto
                {
                    Id = r.Id, EndDate = r.EndDate,
                           StartDate = r.StartDate,
                           Status = r.Status,
                           TotalFare = r.TotalFare,
                    CarId = r.Car.Id,
                    CarNumber = r.Car.Number,
                    CarType = r.Car.Type,
                    CarColor = r.Car.Color,
                    CustomerId = r.Customer.Id,
                    CustomerName = r.Customer.Name,
                    DriverId = r.Driver.Id,
                    DriverName = r.Driver.Name
                }).ToList()
            };*/
           
            // var rentals = await query.ToListAsync();

            //     var rentalPaginationDto = _mapper.Map<RentalPaginationDto>(rentals);
            //    rentalPaginationDto.Count = count;
            return new ApiOkResponse(rentalPaginationDto);

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
            if (createRentalDto.StartDate >= createRentalDto.EndDate)
            {
                return new ApiResponse(400, "Invalid date range. The StartDate must be before the EndDate.");
            }
            if (createRentalDto.StartDate <= DateTime.Now)
            {
                return new ApiResponse(400, "Invalid StartDate. The StartDate must be after the current date.");
            }
            var carExistence = await _carService.IsExistAsync(createRentalDto.CarId);
                if (!carExistence)
                {
                    return new ApiResponse(400, "Invalid car id specified, no car have this id");
                }

            if (createRentalDto.DriverId != null)
            {

                var driverExistence = await _driverService.IsExistAsync(createRentalDto.DriverId.Value);
                if (!driverExistence)
                {
                    return new ApiResponse(400, "Invalid driver id specified, no driver have this id");
                }
             
              
                var availableDriverId = await GetAvailableDriver(createRentalDto.DriverId.Value);

                createRentalDto.DriverId = availableDriverId;
            }
            var customerExistence = await _customerService.IsExistAsync(createRentalDto.CustomerId);
            if (!customerExistence)
            {
                return new ApiResponse(400, "Invalid customer id specified, no customer have this id");
            }
            var carIsAvailable = await _carService.IsAvailableAsync(createRentalDto.CarId);
            if (!carIsAvailable)
            {
                return new ApiResponse(400, "Car is not available for renting");
            }
            if (createRentalDto.StartDate.Date == DateTime.Now.Date)
            {
                _carService.ChangeStatusToNotAvailable(createRentalDto.CarId);
            }
            Rental toCreateRental =  _mapper.Map<Rental>(createRentalDto);
         
            try
            {
                var createdRental = await   _rentalService.AddAsync(toCreateRental);
                var rentalDto = _mapper.Map<RentalDto>(createdRental);
                return new ApiOkResponse(rentalDto);
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
            if (updateRentalDto.StartDate >= updateRentalDto.EndDate)
            {
                return new ApiResponse(400, "Invalid date range. The StartDate must be before the EndDate.");
            }
            if (updateRentalDto.StartDate <= DateTime.Now)
            {
                return new ApiResponse(400, "Invalid StartDate. The StartDate must be after the current date.");
            }
            var carExistence = await _carService.IsExistAsync(updateRentalDto.CarId);
            if (!carExistence)
            {
                return new ApiResponse(400, "Invalid car id specified, no car have this id");
            }
            if(updateRentalDto.DriverId != null) { 
            var driverExistence = await _driverService.IsExistAsync(updateRentalDto.DriverId.Value);
            if (!driverExistence)
            {
                return new ApiResponse(400, "Invalid driver id specified, no driver have this id");
            }
            }
            var customerExistence = await _customerService.IsExistAsync(updateRentalDto.CustomerId);
            if (!customerExistence)
            {
                return new ApiResponse(400, "Invalid customer id specified, no customer have this id");
            }
            var carIsAvailable = await _carService.IsAvailableAsync(updateRentalDto.CarId);
            if (!carIsAvailable)
            {
                return new ApiResponse(400, "Car is not available for renting");
            }
            if (updateRentalDto.StartDate.Date == DateTime.Now.Date)
            {
                _carService.ChangeStatusToNotAvailable(updateRentalDto.CarId);
            }
            var availableDriverId = await GetAvailableDriver(updateRentalDto.DriverId.Value);

            updateRentalDto.DriverId = availableDriverId;
        
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
           
            var IsRentalExist = await _rentalService.IsExistAsync(id);
            if (!IsRentalExist)
            {
                return new ApiResponse(404, "rental not found with id " + id.ToString());
            }
           await _rentalService.DeleteAsync(id);
            return new ApiOkResponse("rental with id" + id + "is deleted");
        }


        [HttpGet("GetAvailableDriver")]
        public async Task<Guid> GetAvailableDriver(Guid id)
        {

            var driver = await _driverService.GetByIdAsync(id);


            if (driver.IsAvailable)
            {
                return id;
            }

            while (driver.ReplacementDriverId != null)
            {
                var replacementDriver = await _driverService.GetByIdAsync(driver.ReplacementDriverId.Value);
                if (replacementDriver == null)
                {
                    break;
                }

                if (replacementDriver.IsAvailable)
                {
                    return replacementDriver.Id;
                }

                driver = replacementDriver;
            }

            throw new Exception("The chossen driver is not availble, and no available replacement driver found.");
        }
      

    }
}
