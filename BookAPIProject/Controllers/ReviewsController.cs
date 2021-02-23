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
    public class ReviewsController : Controller
    {
        IReviewRepository _reviewRepository;
        //TODO:: add book interface
        //IBookRepository _bookRepository;

        public ReviewsController(IReviewRepository reviewRepo) //, IBookRepository bookRepo
        {
            _reviewRepository = reviewRepo;
            //_bookRepository = bookRepo;
        }


        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviews()
        {
            var reviews = _reviewRepository.GetReviews();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dto = new List<ReviewDto>();

            foreach (var review in reviews)
            {
                dto.Add(new ReviewDto() { Id = review.Id, Headline = review.Headline, ReviewText = review.ReviewText, Rating = review.Rating });
            }

            return Ok(dto);
        }


        [HttpGet("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewDto))]
        public IActionResult GetReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var review = _reviewRepository.GetReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dto = new ReviewDto() { Id = review.Id, Headline = review.Headline, ReviewText = review.ReviewText, Rating = review.Rating };


            return Ok(dto);

        }


        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsOfABook(int bookId)
        {
            //TODO:: Validate Book
            //if(!_bookRepository.BookExists(bookId))
            //    return NotFound();

            var reviews = _reviewRepository.GetReviewsOfABook(bookId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dto = new List<ReviewDto>();

            foreach(var review in reviews)
            {
                dto.Add(new ReviewDto() { Id = review.Id, Headline = review.Headline, ReviewText = review.ReviewText, Rating = review.Rating });
            }

            return Ok(dto);
        }


        [HttpGet("{reviewId}/book")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(BookDto))]
        public IActionResult GetBookOfAReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var book = _reviewRepository.GetBookOfAReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dto = new BookDto() { Id = book.ID, Isbn = book.Isbn, Title = book.Title, DatePublished = book.DatePublished };

            return Ok(dto);

        }



    }
}
