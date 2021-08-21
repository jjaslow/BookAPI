﻿using BookAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    public class BookRepository : IBookRepository
    {
        BookDbContext _context;

        public BookRepository(BookDbContext context)
        {
            _context = context;
        }



        public ICollection<Book> GetBooks()
        {
            return _context.Books.OrderBy(b => b.Title).ToList();
        }



        public Book GetBook(int bookId)
        {
            return _context.Books.FirstOrDefault(b => b.ID == bookId);
        }
        public Book GetBook(string Isbn)
        {
            return _context.Books.FirstOrDefault(b => b.Isbn == Isbn);
        }



        public bool BookExists(int bookId)
        {
            return _context.Books.Any(b => b.ID == bookId);
        }

        public bool BookExists(string Isbn)
        {
            return _context.Books.Any(b => b.Isbn == Isbn);
        }




        public bool IsDuplicateIsbn(int bookId, string Isbn)
        {
            bool isbnExists = _context.Books.Any(b => b.Isbn == Isbn);
            if (!isbnExists)
                return false;
            else
            {
                Book existingBook = _context.Books.FirstOrDefault(b => b.Isbn.Trim().ToUpper() == Isbn.Trim().ToUpper());
                return existingBook.ID != bookId;
            }
        }

        public decimal GetBookRating(int bookId)
        {
            var reviews = _context.Reviews.Where(r => r.Book.ID == bookId);

            int numberOfReviews = reviews.Count();

            if(numberOfReviews==0)
                return 0;

            decimal totalRating = 0;

            foreach (var review in reviews)
                totalRating += review.Rating;

            return totalRating / numberOfReviews;
        }



    }
}
