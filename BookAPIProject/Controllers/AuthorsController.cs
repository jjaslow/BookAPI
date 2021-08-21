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

    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : Controller
    {
        IAuthorRepositiory _authorRepository;
        IBookRepository _bookRepository;
        ICountryRepository _countryRepository;
        

        public AuthorsController(IAuthorRepositiory authorRepositiory, IBookRepository bookRepository, ICountryRepository countryRepository) //
        {
            _authorRepository = authorRepositiory;
            _bookRepository = bookRepository;
            _countryRepository = countryRepository;
        }

        ///////////////////////////////////

        //Uri: api/authors
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthors()
        {
            var authors = _authorRepository.GetAuthors();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();

            foreach (var author in authors)
                authorsDto.Add(new AuthorDto {Id = author.Id, FirstName = author.FirstName, LastName = author.LastName });

            return Ok(authorsDto);
        }


        //Uri: api/authors/{authorId}
        [HttpGet("{authorId}", Name = "GetAuthor")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(AuthorDto))]
        public IActionResult GetAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            var author = _authorRepository.GetAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorDto = new AuthorDto { Id = author.Id, FirstName = author.FirstName, LastName = author.LastName };
            return Ok(authorDto);
        }


        //Uri: api/authors/books/{bookId}
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthorsOfABook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var authors = _authorRepository.GetAuthorsOfABook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var authorsDto = new List<AuthorDto>();

            foreach (var author in authors)
                authorsDto.Add(new AuthorDto { Id = author.Id, FirstName = author.FirstName, LastName = author.LastName });

            return Ok(authorsDto);
        }


        //Uri: api/authors/{authorId}/books
        [HttpGet("{authorId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooksByAuthor(int authorId)
        {
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            var books = _authorRepository.GetBooksByAuthor(authorId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();

            foreach (var book in books)
                booksDto.Add(new BookDto
                {
                    Id = book.ID,
                    Isbn = book.Isbn,
                    Title = book.Title,
                    DatePublished = book.DatePublished
                });

            return Ok(booksDto);
        }


        ///////////////////////////////////


        //Uri:  //api/authors
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Author))] //Created
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(500)] //Server Error
        public IActionResult CreateAuthor([FromBody] Author authorToCreate)
        {
            //empty POST data
            if (authorToCreate == null)
                return BadRequest(ModelState);


            //check country is valid
            if(!_countryRepository.CountryExists(authorToCreate.Country.Id))
            {
                ModelState.AddModelError("", "Country doesnt exist.");
                return StatusCode(404, ModelState);
            }


            //assign country (complete object) from the ID submitted
            authorToCreate.Country = _countryRepository.GetCountry(authorToCreate.Country.Id);


            //General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            //actually perform the creation and check for errors
            //error saving new country to db (ie SaveChanges int <0)
            if (!_authorRepository.CreateAuthor(authorToCreate))
            {
                ModelState.AddModelError("", "Something went wrong saving new author");
                return StatusCode(500, ModelState);
            }

            //success (response code 201 - created)
            //Then call back to the GetCountry call above to return the newly created country
            //Since GetCategory has an argument, we need to pass that by creating an anonymous object with the same argument name.
            //We also need to add the 'name' of the method to the HttpGet above (so it can be called by name internally). And finally, we pass the newly created object.
            return CreatedAtRoute("GetAuthor", new { authorId = authorToCreate.Id }, authorToCreate);
        }



        //URI:  //api/authors/{authorId}
        [HttpPut("{authorId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(500)] //Server Error
        public IActionResult UpdateAuthor(int authorId, [FromBody] Author authorToUpdate)
        {
            //empty PUT data
            if (authorToUpdate == null)
                return BadRequest(ModelState);

            ////mis matched Ids
            if (authorId != authorToUpdate.Id)
                return BadRequest(ModelState);

            ////ensure author ID actually exists
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            //check country is valid
            if (!_countryRepository.CountryExists(authorToUpdate.Country.Id))
            {
                ModelState.AddModelError("", "Country doesnt exist.");
                return StatusCode(404, ModelState);
            }


            //assign country (complete object) from the ID submitted
            authorToUpdate.Country = _countryRepository.GetCountry(authorToUpdate.Country.Id);


            ////General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //error saving new category to db (ie SaveChanges int <0)
            if (!_authorRepository.UpdateAuthor(authorToUpdate))
            {
                ModelState.AddModelError("", "Something went wrong updating the author");
                return StatusCode(500, ModelState);
            }

            //success 
            //dont usually return anything on an update
            return NoContent();
        }


        //URI:  //api/authors/{authorId}
        [HttpDelete("{authorId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(409)] //conflict
        [ProducesResponseType(500)] //Server Error
        public IActionResult DeleteAuthor(int authorId)
        {
            ////ensure Author ID actually exists
            if (!_authorRepository.AuthorExists(authorId))
                return NotFound();

            var authorToDelete = _authorRepository.GetAuthor(authorId);

            //need to check if any books belong to the author (cant delete an author that has books)
            //if so, flag an error
            if (_authorRepository.GetBooksByAuthor(authorId).Count > 0)
            {
                ModelState.AddModelError("", "Author cannot be deleted because they have at least 1 book.");
                return StatusCode(409, ModelState);
            }

            ////General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //error deleting
            if (!_authorRepository.DeleteAuthor(authorToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting the author}");
                return StatusCode(500, ModelState);
            }

            //success 
            //dont usually return anything on an update
            return NoContent();
        }




    }
}
