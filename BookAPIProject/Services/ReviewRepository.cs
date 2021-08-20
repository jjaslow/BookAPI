using BookAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    public class ReviewRepository : IReviewRepository
    {
        BookDbContext _context;

        public ReviewRepository(BookDbContext context)
        {
            _context = context;
        }


        ///////////////////////////////////

        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.OrderBy(r => r.Headline).ToList();
        }

        public Review GetReview(int reviewId)
        {
            return _context.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }


        public Book GetBookOfAReview(int reviewId)
        {
            var bookId = _context.Reviews.Where(i => i.Id == reviewId).Select(b => b.Book.ID).FirstOrDefault();
            return _context.Books.Where(i => i.ID == bookId).FirstOrDefault();
        }

        public ICollection<Review> GetReviewsOfABook(int bookId)
        {
            return _context.Reviews.Where(i => i.Book.ID == bookId).ToList();
        }





        public bool ReviewExists(int reviewId)
        {
            return _context.Reviews.Any(r => r.Id == reviewId);
        }


        ///////////////////////////////////



        public bool CreateReview(Review review)
        {
            _context.Add(review);
            return Save();
        }

        public bool UpdateReview(Review review)
        {
            _context.Update(review);
            return Save();
        }

        public bool DeleteReview(Review review)
        {
            _context.Remove(review);
            return Save();
        }

        public bool Save()
        {
            int result = _context.SaveChanges();
            return result >= 0;
        }
    }
}
