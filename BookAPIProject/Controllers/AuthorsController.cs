using BookAPIProject.Dtos;
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

        public AuthorsController(IAuthorRepositiory authorRepositiory, IBookRepository bookRepository) //
        {
            _authorRepository = authorRepositiory;
            _bookRepository = bookRepository;
        }


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
        [HttpGet("{authorId}")]
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



    }
}
