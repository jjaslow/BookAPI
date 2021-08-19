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
    public class BooksController : Controller
    {
        IBookRepository _bookRepository;
        public BooksController(IBookRepository bookRepo)
        {
            _bookRepository = bookRepo;
        }



        //Uri: api/books
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooks()
        {
            var books = _bookRepository.GetBooks();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();
            foreach (var book in books)
                booksDto.Add(new BookDto { DatePublished = book.DatePublished, Id = book.ID, Isbn = book.Isbn, Title = book.Title });

            return Ok(booksDto);
        }


        //Uri: api/books/{bookId}
        [HttpGet("{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBook(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var book = _bookRepository.GetBook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto { DatePublished = book.DatePublished, Id = book.ID, Isbn = book.Isbn, Title = book.Title };

            return Ok(bookDto);
        }


        //Uri: api/books/ISBN/{Isbn}
        [HttpGet("ISBN/{Isbn}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBook(string Isbn)
        {
            if (!_bookRepository.BookExists(Isbn))
                return NotFound();

            var book = _bookRepository.GetBook(Isbn);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var bookDto = new BookDto { DatePublished = book.DatePublished, Id = book.ID, Isbn = book.Isbn, Title = book.Title };

            return Ok(bookDto);
        }


        //Uri: api/books/{bookId}/rating
        [HttpGet("{bookId}/rating")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(decimal))]
        public IActionResult GetBookRating(int bookId)
        {
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var rating = _bookRepository.GetBookRating(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(rating);
        }




    }
}
