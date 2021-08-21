using BookAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    public class ReviewerRepository : IReviewerRepository
    {

        BookDbContext _context;

        public ReviewerRepository(BookDbContext context)
        {
            _context = context;
        }

        ///////////////////////////////////

        public ICollection<Reviewer> GetReviewers()
        {
            return _context.Reviewers.OrderBy(r => r.LastName).ToList();
        }


        public Reviewer GetReviewer(int reviewerId)
        {
            return _context.Reviewers.Where(r => r.Id == reviewerId).FirstOrDefault();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return _context.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
        }


        public Reviewer GetReviewerOfAReview(int reviewId)
        {
            var reviewerId = _context.Reviews.Where(r => r.Id == reviewId).Select(r=> r.Reviewer.Id).FirstOrDefault();
            return _context.Reviewers.FirstOrDefault(r => r.Id == reviewerId);
        }



        public bool ReviewerExists(int reviewerId)
        {
            return _context.Reviewers.Any(r => r.Id == reviewerId);
        }



        ///////////////////////////////////

        public bool CreateReviewer(Reviewer reviewer)
        {
            _context.Add(reviewer);
            return Save();
        }

        public bool UpdateReviewer(Reviewer reviewer)
        {
            _context.Update(reviewer);
            return Save();
        }

        public bool DeleteReviewer(Reviewer reviewer)
        {
            _context.Remove(reviewer);
            return Save();
        }


        public bool Save()
        {
            int result = _context.SaveChanges();
            return result >= 0;
        }

    }
}
