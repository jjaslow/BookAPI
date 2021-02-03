using BookAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    interface IBookRepository
    {
        ICollection<Book> GetBooks();
        Book GetBook(int bookId);
        Book GetBook(string Isbn);
        bool BookExists(int bookId);
        bool BookExists(string Isbn);


        bool IsDuplicateIsbn(string Isbn);
        decimal GetBookRating(int bookId);

    }
}
