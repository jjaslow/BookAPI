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
    public class BooksController : Controller
    {
        IBookRepository _bookRepository;
        IAuthorRepositiory _authorRepository;
        ICategoryRepository _categoryRepository;
        IReviewRepository _reviewRepository;
        public BooksController(IBookRepository bookRepo, IAuthorRepositiory authorRepository, ICategoryRepository categoryRepository, IReviewRepository reviewRepository)
        {
            _bookRepository = bookRepo;
            _authorRepository = authorRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
        }

        ///////////////////////////////////

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
        [HttpGet("{bookId}", Name = "GetBook")]
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


        ///////////////////////////////////


        //break out validation checks for data
        private StatusCodeResult ValidateBook(Book book, List<int> authorIds, List<int> categoryIds)
        {
            //empty Book data. Also each book needs at least 1 author and category
            if (book==null || authorIds.Count==0 || categoryIds.Count==0)
            {
                ModelState.AddModelError("", "Missing book, author, or category");
                return BadRequest();
            }

            //check for duplicate book
            if(_bookRepository.IsDuplicateIsbn(book.ID, book.Isbn))
            {
                ModelState.AddModelError("", "Duplicate ISBN");
                return StatusCode(422);
            }

            //are cat and author IDs valid
            foreach(var authorId in authorIds)
            {
                if(!_authorRepository.AuthorExists(authorId))
                {
                    ModelState.AddModelError("", "Author not found");
                    return StatusCode(404);
                }
            }
            foreach (var categoryId in categoryIds)
            {
                if (!_categoryRepository.CategoryExists(categoryId))
                {
                    ModelState.AddModelError("", "Category not found");
                    return StatusCode(404);
                }
            }

            //General ModelState error
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Critical error");
                return BadRequest();
            }

            //valid status
            return NoContent();
        }


        //Uri:  //api/books?authId=1&authId=2&catId=1&catId=2
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Book))] //Created
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(422)] //Unprocessable Entity
        [ProducesResponseType(500)] //Server Error
        //the [FromQuery] List name MUST be the same as the query string in the URI
        public IActionResult CreateBook([FromBody] Book bookToCreate, [FromQuery]List<int> authId, [FromQuery] List<int> catId)
        {
            //validate data using private method. And return code if its an error
            var statusCode = ValidateBook(bookToCreate, authId, catId);
            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);


            //actually perform the creation and check for errors
            //error saving new book to db (ie SaveChanges int <0)
            if (!_bookRepository.CreateBook(bookToCreate, authId, catId))
            {
                ModelState.AddModelError("", "Something went wrong saving new book");
                return StatusCode(500, ModelState);
            }

            //success (response code 201 - created)
            //Then call back to the GetCountry call above to return the newly created country
            //Since GetCategory has an argument, we need to pass that by creating an anonymous object with the same argument name.
            //We also need to add the 'name' of the method to the HttpGet above (so it can be called by name internally). And finally, we pass the newly created object.
            return CreatedAtRoute("GetBook", new { bookId = bookToCreate.ID }, bookToCreate);
        }


        //Uri:  //api/books/bookId?authId=1&authId=2&catId=1&catId=2
        [HttpPut("{bookId}")]
        [ProducesResponseType(204)] //No Content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(422)] //Unprocessable Entity
        [ProducesResponseType(500)] //Server Error
        //the [FromQuery] List name MUST be the same as the query string in the URI
        public IActionResult UpdateBook(int bookId, [FromBody] Book bookToUpdate, [FromQuery] List<int> authId, [FromQuery] List<int> catId)
        {

            ////mis matched Ids
            if (bookId != bookToUpdate.ID)
                return BadRequest(ModelState);

            ////ensure author ID actually exists
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            //validate data using private method. And return code if its an error
            var statusCode = ValidateBook(bookToUpdate, authId, catId);
            if (!ModelState.IsValid)
                return StatusCode(statusCode.StatusCode);


            //actually perform the creation and check for errors
            //error saving new book to db (ie SaveChanges int <0)
            if (!_bookRepository.UpdateBook(bookToUpdate, authId, catId))
            {
                ModelState.AddModelError("", "Something went wrong updating the book");
                return StatusCode(500, ModelState);
            }

            //success 
            return NoContent();
        }



        //URI:  //api/books/{bookId}
        [HttpDelete("{bookId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(500)] //Server Error
        public IActionResult DeleteBook(int bookId)
        {
            ////ensure book ID actually exists
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var bookToDelete = _bookRepository.GetBook(bookId);
            var reviewsToDelete = _reviewRepository.GetReviewsOfABook(bookId);

            ////General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //error deleting reviews
            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", "Something went wrong deleting the book's reviews.");
                return StatusCode(500, ModelState);
            }

            //error deleting book
            if (!_bookRepository.DeleteBook(bookToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting the book.");
                return StatusCode(500, ModelState);
            }

            //success 
            //dont usually return anything on an update
            return NoContent();
        }


    }
}
