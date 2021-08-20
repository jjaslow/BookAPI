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
    public class ReviewsController : Controller
    {
        IReviewRepository _reviewRepository;
        IBookRepository _bookRepository;
        IReviewerRepository _reviewerRepository;

        public ReviewsController(IReviewRepository reviewRepo, IBookRepository bookRepo, IReviewerRepository reviewerRepo) //, 
        {
            _reviewRepository = reviewRepo;
            _bookRepository = bookRepo;
            _reviewerRepository = reviewerRepo;
        }

        ////////////////////////////////////////////

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


        [HttpGet("{reviewId}", Name = "GetReview")]
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
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

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


        ////////////////////////////////////////////

        //Uri:  //api/reviews
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Review))] //Created
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(500)] //Server Error
        public IActionResult CreateReview([FromBody]Review newReview)
        {
            //empty POST data
            if (newReview == null)
                return BadRequest(ModelState);


            //verify that Reviewer and Book are valid
            if(!_reviewerRepository.ReviewerExists(newReview.Reviewer.Id))
                ModelState.AddModelError("", "Reviewer doesnt exist.");
            if(!_bookRepository.BookExists(newReview.Book.ID))
                ModelState.AddModelError("", "Book doesnt exist.");
            //ModelState error for missing data of reviewer or book
            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);

            //assign book and reviewer (complete objects) from the IDs submitted
            newReview.Book = _bookRepository.GetBook(newReview.Book.ID);
            newReview.Reviewer = _reviewerRepository.GetReviewer(newReview.Reviewer.Id);

            //General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //actually perform the creation and check for errors
            //error saving new country to db (ie SaveChanges int <0)
            if (!_reviewRepository.CreateReview(newReview))
            {
                ModelState.AddModelError("", "Something went wrong saving the review.");
                return StatusCode(500, ModelState);
            }

            //success (response code 201 - created)
            //Then call back to the GetCountry call above to return the newly created country
            //Since GetCategory has an argument, we need to pass that by creating an anonymous object with the same argument name.
            //We also need to add the 'name' of the method to the HttpGet above (so it can be called by name internally). And finally, we pass the newly created object.
            return CreatedAtRoute("GetReview", new { reviewId = newReview.Id }, newReview);
        }




        //URI:  //api/reviews/{reviewId}
        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(500)] //Server Error
        public IActionResult UpdateReview(int reviewId, [FromBody] Review reviewToUpdate)
        {
            //empty PUT data
            if (reviewToUpdate == null)
                return BadRequest(ModelState);

            ////mis matched Ids
            if (reviewId != reviewToUpdate.Id)
                return BadRequest(ModelState);

            ////ensure review ID actually exists
            if (!_reviewRepository.ReviewExists(reviewId))
                ModelState.AddModelError("", "Review doesnt exist.");

            //verify that Reviewer and Book are valid
            if (!_reviewerRepository.ReviewerExists(reviewToUpdate.Reviewer.Id))
                ModelState.AddModelError("", "Reviewer doesnt exist.");
            if (!_bookRepository.BookExists(reviewToUpdate.Book.ID))
                ModelState.AddModelError("", "Book doesnt exist.");
            //ModelState error for missing data of reviewer or book or reviewID
            if (!ModelState.IsValid)
                return StatusCode(404, ModelState);


            //assign book and reviewer (complete objects) from the IDs submitted
            reviewToUpdate.Book = _bookRepository.GetBook(reviewToUpdate.Book.ID);
            reviewToUpdate.Reviewer = _reviewerRepository.GetReviewer(reviewToUpdate.Reviewer.Id);


            ////General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //error saving new category to db (ie SaveChanges int <0)
            if (!_reviewRepository.UpdateReview(reviewToUpdate))
            {
                ModelState.AddModelError("", "Something went wrong updating the review");
                return StatusCode(500, ModelState);
            }

            //success 
            //dont usually return anything on an update
            return NoContent();
        }



        //URI:  //api/reviews/{reviewId}
        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(500)] //Server Error
        public IActionResult DeleteReview(int reviewId)
        {
            ////ensure Review ID actually exists
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var reviewToDelete = _reviewRepository.GetReview(reviewId);

            ////General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //error deleting
            if (!_reviewRepository.DeleteReview(reviewToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting the review.");
                return StatusCode(500, ModelState);
            }

            //success 
            //dont usually return anything on an update
            return NoContent();
        }


    }
}
