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
        IAuthorRepositiory _authorRepositiory;

        public CountriesController(ICountryRepository countryRepository, IAuthorRepositiory authorRepositiory)
        {
            _countryRepository = countryRepository;
            _authorRepositiory = authorRepositiory;
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
            //TODO:: validate that author exists...test
            if (!_authorRepositiory.AuthorExists(authorId))
                return NotFound();

            var country = _countryRepository.GetCountryOfAnAuthor(authorId);

            //wont need after we validate author exists
            //if (country == null)
            //    return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryDto = new CountryDto { Id = country.Id, Name = country.Name };

            return Ok(countryDto);
        }



        //Uri: api/countries/{countryId}/authors
        //dont need to manually enter the leading slash. It knows
        [HttpGet("{countryId}/authors")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthorsFromACountry(int countryId)
        {
            if(!_countryRepository.CountryExists(countryId))
                return NotFound();

            var authors = _countryRepository.GetAuthorsFromACountry(countryId);

            //wont need after we validate country exists
            //if (authors.ToList().Count == 0)
            //    return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();
            foreach (var author in authors)
                authorsDto.Add(new AuthorDto { Id = author.Id, FirstName = author.FirstName, LastName = author.LastName });


            return Ok(authorsDto);
        }

    }


}


