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
    public class ReviewersController : Controller
    {
        IReviewerRepository _reviewerRepository;
        IReviewRepository _reviewRepository;

        public ReviewersController(IReviewerRepository reviewerRepository, IReviewRepository reviewRepository)
        {
            _reviewerRepository = reviewerRepository;
            _reviewRepository = reviewRepository;
        }

        ///////////////////////////////////

        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewerDto>))]
        public IActionResult GetReviewers()
        {
            var reviewers = _reviewerRepository.GetReviewers();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewersDto = new List<ReviewerDto>();
            foreach (var reviewer in reviewers)
                reviewersDto.Add(new ReviewerDto { Id=reviewer.Id, FirstName=reviewer.FirstName, LastName=reviewer.LastName });

            return Ok(reviewersDto);
        }


        [HttpGet("{reviewerId}", Name = "GetReviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviewer = _reviewerRepository.GetReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            ReviewerDto reviewerDto = new ReviewerDto {Id=reviewer.Id, FirstName=reviewer.FirstName, LastName=reviewer.LastName };

            return Ok(reviewerDto);
        }


        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<ReviewDto>))]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewsDto = new List<ReviewDto>();
            foreach (var review in reviews)
                reviewsDto.Add(new ReviewDto {Id=review.Id, Headline=review.Headline, ReviewText = review.ReviewText, Rating = review.Rating });

            return Ok(reviewsDto);
        }



        [HttpGet("{reviewId}/reviewer")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(ReviewerDto))]
        public IActionResult GetReviewerOfAReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var reviewer = _reviewerRepository.GetReviewerOfAReview(reviewId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewerDto = new ReviewerDto { Id = reviewer.Id, FirstName = reviewer.FirstName, LastName = reviewer.LastName };

            return Ok(reviewerDto);
        }


        ///////////////////////////////////



        //Uri:  //api/reviewers
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Reviewer))] //Created
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(500)] //Server Error
        public IActionResult CreateReviewer([FromBody] Reviewer reviewerToCreate)
        {
            //empty POST data
            if (reviewerToCreate == null)
                return BadRequest(ModelState);

            //General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //actually perform the creation and check for errors
            //error saving new country to db (ie SaveChanges int <0)
            if (!_reviewerRepository.CreateReviewer(reviewerToCreate))
            {
                ModelState.AddModelError("", "Something went wrong saving new reviewer");
                return StatusCode(500, ModelState);
            }

            //success (response code 201 - created)
            //Then call back to the GetCountry call above to return the newly created country
            //Since GetCategory has an argument, we need to pass that by creating an anonymous object with the same argument name.
            //We also need to add the 'name' of the method to the HttpGet above (so it can be called by name internally). And finally, we pass the newly created object.
            return CreatedAtRoute("GetReviewer", new { reviewerId = reviewerToCreate.Id }, reviewerToCreate);
        }


        //URI:  //api/reviewers/{reviewerId}
        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(500)] //Server Error
        public IActionResult UpdateReviewer(int reviewerId, [FromBody] Reviewer reviewerToUpdate)
        {
            //empty PUT data
            if (reviewerToUpdate == null)
                return BadRequest(ModelState);

            ////mis matched Ids
            if (reviewerId != reviewerToUpdate.Id)
                return BadRequest(ModelState);

            ////ensure reviewer ID actually exists
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();


            ////General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //error saving new category to db (ie SaveChanges int <0)
            if (!_reviewerRepository.UpdateReviewer(reviewerToUpdate))
            {
                ModelState.AddModelError("", "Something went wrong updating the reviewer");
                return StatusCode(500, ModelState);
            }

            //success 
            //dont usually return anything on an update
            return NoContent();
        }


        //URI:  //api/reviewers/{reviewerId}
        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(500)] //Server Error
        public IActionResult DeleteReviewer(int reviewerId)
        {
            ////ensure Review ID actually exists
            if (!_reviewerRepository.ReviewerExists(reviewerId))
                return NotFound();

            var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);
            var reviewsToDelete = _reviewerRepository.GetReviewsByReviewer(reviewerId);

            ////General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //error deleting reviewer's reviews
            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", "Something went wrong deleting the reviewers reviews.");
                return StatusCode(500, ModelState);
            }

            //error deleting reviewer
            if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting the reviewer.");
                return StatusCode(500, ModelState);
            }

            //success 
            //dont usually return anything on an update
            return NoContent();
        }


    }



}
