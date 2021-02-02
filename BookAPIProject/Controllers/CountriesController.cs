using BookAPIProject.Dtos;
using BookAPIProject.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : Controller
    {
        ICountryRepository _countryRepository;

        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }



        //Uri: api/countries
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = _countryRepository.GetCountries().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countriesDto = new List<CountryDto>();
            foreach (var country in countries)
                countriesDto.Add(new CountryDto { Id = country.Id, Name = country.Name });

            return Ok(countriesDto);
        }


        //Uri: api/countries/{countryId}
        //dont need to manually enter the leading slash. It knows
        [HttpGet("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var country = _countryRepository.GetCountry(countryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new CountryDto {Id = country.Id, Name = country.Name};

            return Ok(countryDto);
        }


        //Uri: api/countries/{countryId}
        //dont need to manually enter the leading slash. It knows
        [HttpGet("authors/{authorId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountryOfAnAnthor(int authorId)
        {
            //TODO:: validate that author exists

            var country = _countryRepository.GetCountryOfAnAuthor(authorId);

            //wont need after we validate author exists
            if (country == null)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new CountryDto { Id = country.Id, Name = country.Name };

            return Ok(countryDto);
        }



    }


}
