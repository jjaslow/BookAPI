using BookAPIProject.Models;
using Microsoft.EntityFrameworkCore;
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
        ///////////////////////////////////


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
            //bool isbnExists = _context.Books.Any(b => b.Isbn == Isbn);
            //if (!isbnExists)
            //    return false;
            //else
            //{
            //    Book existingBook = _context.Books.AsNoTracking().FirstOrDefault(b => b.Isbn.Trim().ToUpper() == Isbn.Trim().ToUpper());
            //    return existingBook.ID != bookId;
            //}

            var book = _context.Books.Where(b => b.Isbn.Trim().ToUpper() == Isbn.Trim().ToUpper() && b.ID != bookId).FirstOrDefault();
            return book == null ? false : true;

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



        ///////////////////////////////////

            

        public bool CreateBook(Book book, List<int> authorIds, List<int> categoryIds)
        {
            //find all the authors (from Authors table) in the supplied list of Ids and add a new BookAuthor entry linking book to author
            foreach(var author in _context.Authors.Where(a => authorIds.Contains(a.Id)))
            {
                _context.Add(new BookAuthor 
                {   
                    Author = author,
                    Book = book,

                    AuthorId = author.Id,
                    BookId = book.ID
                });
            }

            //find all the categories (from Categories table) in the supplied list of Ids and add a new CategoryAuthor entry linking book to category
            foreach (var category in _context.Categories.Where(c => categoryIds.Contains(c.Id)))
            {
                _context.Add(new BookCategory
                {

                    Category = category,
                    Book = book,

                    CategoryId = category.Id,
                    BookId = book.ID
                });
            }

            _context.Add(book);
            return Save();
        }


        public bool UpdateBook(Book book, List<int> authorIds, List<int> categoryIds)
        {
            //first remove all BookAuthors for this book, so we can add fresh
            var bookAuthorsToRemove = _context.BookAuthors.Where(b => b.BookId==book.ID);
            _context.RemoveRange(bookAuthorsToRemove);

            //find all the authors (from Authors table) in the supplied list of Ids and add a new BookAuthor entry linking book to author
            foreach (var author in _context.Authors.Where(a => authorIds.Contains(a.Id)))
            {
                _context.Add(new BookAuthor
                {
                    Author = author,
                    Book = book,

                    AuthorId = author.Id,
                    BookId = book.ID
                });
            }



            //first remove all CategoryAuthors for this book, so we can add fresh
            var bookCategoriesToRemove = _context.BookCategories.Where(b => b.BookId == book.ID);
            _context.RemoveRange(bookCategoriesToRemove);

            //find all the categories (from Categories table) in the supplied list of Ids and add a new CategoryAuthor entry linking book to category
            foreach (var category in _context.Categories.Where(c => categoryIds.Contains(c.Id)))
            {
                _context.Add(new BookCategory
                {

                    Category = category,
                    Book = book,

                    CategoryId = category.Id,
                    BookId = book.ID
                });
            }



            _context.Update(book);
            return Save();
        }


        public bool DeleteBook(Book book)
        {
            _context.Remove(book);
            return Save();
        }


        public bool Save()
        {
            int result = _context.SaveChanges();
            return result >= 0;
        }





    }
}
