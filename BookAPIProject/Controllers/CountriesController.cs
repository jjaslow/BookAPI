using BookAPIProject.Dtos;
using BookAPIProject.Models;
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


        ////////////////////////////////////////////

        //Uri: api/countries
        [HttpGet]
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(200, Type = typeof(IEnumerable<CountryDto>))]
        public IActionResult GetCountries()
        {
            var countries = _countryRepository.GetCountries();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countriesDto = new List<CountryDto>();
            foreach (var country in countries)
                countriesDto.Add(new CountryDto { Id = country.Id, Name = country.Name });

            return Ok(countriesDto);
        }


        //Uri: api/countries/{countryId}
        //dont need to manually enter the leading slash. It knows
        //need to explecitly enter the attribute name of the method in the action for internal calling
        [HttpGet("{countryId}", Name = "GetCountry")]
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
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
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(200, Type = typeof(CountryDto))]
        public IActionResult GetCountryOfAnAnthor(int authorId)
        {
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
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
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




        ////////////////////////////////////////////

        //URI: //api/countries
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Country))] //Created
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(422)] //unprocessable
        [ProducesResponseType(500)] //Server Error
        public IActionResult CreateCountry([FromBody]Country countryToCreate)
        {
            //empty POST data
            if (countryToCreate == null)
                return BadRequest(ModelState);

            //new countryToCreate Name already exists
            if (_countryRepository.GetCountries()
                .Any(c => c.Name.Trim().ToUpper() == countryToCreate.Name.Trim().ToUpper()))
            {
                ModelState.AddModelError("", $"Country {countryToCreate.Name} already exists");
                return StatusCode(422, ModelState);
            }

            //General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //actually perform the creation and check for errors
            //error saving new country to db (ie SaveChanges int <0)
            if(!_countryRepository.CreateCountry(countryToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {countryToCreate.Name}");
                return StatusCode(500, ModelState);
            }

            //success (response code 201 - created)
            //Then call back to the GetCountry call above to return the newly created country
            //Since GetCategory has an argument, we need to pass that by creating an anonymous object with the same argument name.
            //We also need to add the 'name' of the method to the HttpGet above (so it can be called by name internally). And finally, we pass the newly created object.
            return CreatedAtRoute("GetCountry", new { countryId = countryToCreate.Id}, countryToCreate);
        }


        //URI:  //api/countries/{countryId}
        //we have the countryId twice in the arguments (one included in the Country), but do so to 2x check where we can confirm the Id in the Country object matches the int whic will be passed in the URL
        [HttpPut("{countryId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(422)] //unprocessable
        [ProducesResponseType(500)] //Server Error
        public IActionResult UpdateCountry(int countryId, [FromBody]Country updatedCountryInfo)
        {
            //empty PUT data
            if (updatedCountryInfo == null)
                return BadRequest(ModelState);

            ////mis matched Ids
            if (countryId != updatedCountryInfo.Id)
                return BadRequest(ModelState);

            ////ensure country ID actually exists
            ///if (!_countryRepository.GetCountries().Any(c => c.Id == updatedCountryInfo.Id))
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            ////validate that we arent trying to update this country with a pre-existing name
            if (_countryRepository.IsDuplicateCountryName(countryId, updatedCountryInfo.Name))
            {
                ModelState.AddModelError("", $"Country {updatedCountryInfo.Name} already exists");
                return StatusCode(422, ModelState);
            }

            ////General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //error saving new country to db (ie SaveChanges int <0)
            if (!_countryRepository.UpdateCountry(updatedCountryInfo))
            {
                ModelState.AddModelError("", $"Something went wrong updating {updatedCountryInfo.Name}");
                return StatusCode(500, ModelState);
            }

            //success 
            //dont usually return anything on an update
            return NoContent();

        }


        //URI:  //api/countries/{countryId}
        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(409)] //conflict
        [ProducesResponseType(500)] //Server Error
        public IActionResult DeleteCountry(int countryId)
        {
            ////ensure country ID actually exists
            if (!_countryRepository.CountryExists(countryId))
                return NotFound();

            var countryToDelete = _countryRepository.GetCountry(countryId);

            //need to check if any authors are using this country (cant delete a country being used by an author)
            //if so, flag an error
            if(_countryRepository.GetAuthorsFromACountry(countryId).Count>0)
            {
                ModelState.AddModelError("", $"Country {countryToDelete.Name} cannot be deleted because it is used by at least 1 author");
                return StatusCode(409, ModelState);
            }

            ////General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //error deleting
            if(!_countryRepository.DeleteCountry(countryToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {countryToDelete.Name}");
                return StatusCode(500, ModelState);
            }

            //success 
            //dont usually return anything on an update
            return NoContent();
        }


    }


}


