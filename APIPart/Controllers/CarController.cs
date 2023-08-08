﻿using AutoMapper;
using Core.Entities;
using Core.enums;
using Core.Interfaces;
using Core.Models;
using APIPart.DTOs.CarDtos;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using APIPart.DTOs;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CarController : Controller
    {
        private IGenericRepository<Car> _carRepository;

        private readonly IMapper _mapper;
        public CarController(IGenericRepository<Car> carRepository, IMapper mapper) {
            _carRepository = carRepository;
            _mapper = mapper;
        }

        [Route("GetCars")]

        [HttpGet]
        public CarPaginationDto GetCars([FromQuery] ListRequestDto listRequestDto)
        {
            var searchWord = listRequestDto.SearchWord.ToLower();

            var query = _carRepository.GetQueryable()
                .Where(c =>
                    c.Number.ToLower().Contains(searchWord) ||
                    c.Type.ToLower().Contains(searchWord) ||
                    c.Color.ToLower().Contains(searchWord) ||
                    c.DailyFare.ToString().Contains(searchWord) ||
                    c.HasDriver.ToString().ToLower().Contains(searchWord) ||
                    c.IsAvailable.ToString().ToLower().Contains(searchWord) ||
                    (c.DriverId != null && c.DriverId.ToString().Contains(searchWord))
                );
            var count = query.Count();
            if (listRequestDto.SortingType == SortingType.asc)
            {
                query = query.OrderBy(c => c.Number);
            }
            else if (listRequestDto.SortingType == SortingType.desc)
            {
                query = query.OrderByDescending(c => c.Number);
            }


            /*
            var query = _carRepository.GetQueryable().
                Where(x =>x.Color.ToLower().Contains(carRequestDto.SearchWord.ToLower()));*/
        
            //       query = query.Skip(carRequestDto.PageNumber).Take(carRequestDto.PageSize);
            var pageIndex = listRequestDto.PageNumber - 1; 
            var pageSize = listRequestDto.PageSize;

            query = query.Skip(pageIndex * pageSize).Take(pageSize);
            var cars = query.ToList();
   
            var carPaginationDto = _mapper.Map<CarPaginationDto>(cars);
            carPaginationDto.Count = count;
            return carPaginationDto;
        }
        [HttpGet]
        public IEnumerable<CarListDto> GetList()
        {
            var cars = _carRepository.
                        GetAll();
            IEnumerable<CarListDto>? carListDto = _mapper.Map<IEnumerable<CarListDto>>(cars);
            return carListDto;
        }
        [HttpGet("{id}")]
        public CarDTO Get(Guid id)
        {
            var car = _carRepository.GetById(id);

            CarDTO carDto = _mapper.Map<CarDTO>(car);
            return carDto;
        }
        [HttpPost]
        public CreateCarDto Create(CreateCarDto createCarDto)
        {/*
            var car = new Car
            {
                Number = createCarDto.Number,
                Type = createCarDto.Type,
                EngineCapacity = createCarDto.EngineCapacity,
                Color = createCarDto.Color,
                DailyFare = createCarDto.DailyFare,

                //DriverId = createCarDto.DriverId
            };
            _carRepository.Add(car);
            CreateCarDto carDto = _mapper.Map<CreateCarDto>(car);
            return carDto;*/
            Car toCreateCar= _mapper.Map<Car>(createCarDto);
            _carRepository.Add(toCreateCar);
            return createCarDto;
        }

        [HttpPut("{id}")]
        public UpdateCarDto Update(Guid id, UpdateCarDto updateCarDto)
        {
            /*
            var car = _carRepository.GetById(id);
            if (car == null)
            {
                return null;
            }
            car.Number = updateCarDto.Number;
            car.Type = updateCarDto.Type;
            car.EngineCapacity = updateCarDto.EngineCapacity;
            car.Color = updateCarDto.Color;
            car.DailyFare = updateCarDto.DailyFare;

            car.DriverId = updateCarDto.DriverId;
            _carRepository.Update(id,car);*/
            var car = _carRepository.GetById(id);
            if (car == null)
            {
                return null;
            }
            else {
                
                var newCar = _mapper.Map<Car>(updateCarDto);
                _carRepository.Update(id, newCar);
                return updateCarDto;
            }
           
           
        }
        [HttpDelete("{id}")]
        public CarDTO Delete(Guid id)
        {
            var car = _carRepository.GetById(id);
            if (car == null)
            {
                return null;
            }
            _carRepository.Delete(id);

            CarDTO carDto = _mapper.Map<CarDTO>(car);

            return carDto;
        }



            /*
            private readonly CarService _service;

            public CarController(CarService service)
            {
                _service = service;
            }

            [HttpGet]
            public IActionResult GetAll()
            {
                var cars = _service.
                    GetAllCars().Select(c => new CarDTO
                    {
                        Id = c.Id,
                        Number = c.Number,
                        Type = c.Type,
                        EngineCapacity = c.EngineCapacity,
                        Color = c.Color,
                        DailyFare = c.DailyFare,

                        DriverId = c.DriverId
                    });
                return Ok(cars);
            }

            [HttpGet("{id}")]
            public IActionResult GetById(Guid id)
            {
                var car = _service.GetCarById(id);
                if (car == null)
                {
                    return NotFound();
                }
                var carDTO = new CarDTO
                {
                    Id = car.Id,
                    Number = car.Number,
                    Type = car.Type,
                    EngineCapacity = car.EngineCapacity,
                    Color = car.Color,
                    DailyFare = car.DailyFare,

                    DriverId = car.DriverId
                };
                return Ok(carDTO);
            }

            [HttpPost]
            public IActionResult Create(CarDTO carDTO)
            {
                var car = new Car
                {
                    Number = carDTO.Number,
                    Type = carDTO.Type,
                    EngineCapacity = carDTO.EngineCapacity,
                    Color = carDTO.Color,
                    DailyFare = carDTO.DailyFare,

                    DriverId = carDTO.DriverId
                };
                _service.AddCar(car);
                return CreatedAtAction(nameof(GetById), new { id = car.Id }, car);
            }

            [HttpPut("{id}")]
            public IActionResult Update(Guid id, CarDTO carDTO)
            {
                var car = _service.GetCarById(id);
                if (car == null)
                {
                    return NotFound();
                }
                car.Number = carDTO.Number;
                car.Type = carDTO.Type;
                car.EngineCapacity = carDTO.EngineCapacity;
                car.Color = carDTO.Color;
                car.DailyFare = carDTO.DailyFare;

                car.DriverId = carDTO.DriverId;
                _service.UpdateCar(car);
                return NoContent();
            }

            [HttpDelete("{id}")]
            public IActionResult Delete(Guid id)
            {
                var car = _service.GetCarById(id);
                if (car == null)
                {
                    return NotFound();
                }
                _service.DeleteCar(car);
                return NoContent();
            }
            public IActionResult Index()
            {
                var cars = _service.GetAllCars().Select(c => new CarDTO
                {
                    Id = c.Id,
                    Number = c.Number,
                    Type = c.Type,
                    EngineCapacity = c.EngineCapacity,
                    Color = c.Color,
                    DailyFare = c.DailyFare,
                    DriverId = c.DriverId
                }).ToList();

                return NoContent();
            }
            */

        }
}



