using APIPart.DTOs.DriverDtos;
using APIPart.DTOs;
using AutoMapper;
using Core.Entities;
using Core.enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriverController : Controller
    {
        private IGenericRepository<Driver> _driverRepository;

        private readonly IMapper _mapper;
        public DriverController(IGenericRepository<Driver> driverRepository, IMapper mapper)
        {
            _driverRepository = driverRepository;
            _mapper = mapper;
        }
        [Route("GetDrivers")]

        [HttpGet]
        public DriverPaginationDto GetDrivers([FromQuery] ListRequestDto listRequestDto)
        {
            var searchWord = listRequestDto.SearchWord.ToLower();

            var query = _driverRepository.GetQueryable()
                .Where(c =>
                    c.Name.ToLower().Contains(searchWord) ||
                    c.Phone.ToLower().Contains(searchWord) ||
                    c.LicenseNumber.ToLower().Contains(searchWord)
                );
            var count = query.Count();
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
            var drivers = query.ToList();

            var driverPaginationDto = _mapper.Map<DriverPaginationDto>(drivers);
            driverPaginationDto.Count = count;
            return driverPaginationDto;
        }
        [HttpGet]
        public IEnumerable<DriverListDto> GetList()
        {
            var drivers = _driverRepository.
                        GetAll();
            IEnumerable<DriverListDto>? driverListDto = _mapper.Map<IEnumerable<DriverListDto>>(drivers);
            return driverListDto;
        }
        [HttpGet("{id}")]
        public DriverDto Get(Guid id)
        {
            var driver = _driverRepository.GetById(id);

            DriverDto driverDto = _mapper.Map<DriverDto>(driver);
            return driverDto;
        }
        [HttpPost]
        public CreateDriverDto Create(CreateDriverDto createDriverDto)
        {
            Driver toCreateDriver = _mapper.Map<Driver>(createDriverDto);
            _driverRepository.Add(toCreateDriver);
            return createDriverDto;
        }

        [HttpPut("{id}")]
        public UpdateDriverDto Update(Guid id, UpdateDriverDto updateDriverDto)
        {

            var driver = _driverRepository.GetById(id);
            if (driver == null)
            {
                return null;
            }
            else
            {

                var newDriver = _mapper.Map<Driver>(updateDriverDto);
                _driverRepository.Update(id, newDriver);
                return updateDriverDto;
            }


        }
        [HttpDelete("{id}")]
        public DriverDto Delete(Guid id)
        {
            var driver = _driverRepository.GetById(id);
            if (driver == null)
            {
                return null;
            }
            _driverRepository.Delete(id);

            DriverDto driverDto = _mapper.Map<DriverDto>(driver);

            return driverDto;
        }
    }
}
