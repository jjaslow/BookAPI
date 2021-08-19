using BookAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    public class AuthorRepository : IAuthorRepositiory
    {
        BookDbContext _authorContext;

        public AuthorRepository(BookDbContext context)
        {
            _authorContext = context;
        }





        public ICollection<Author> GetAuthors()
        {
            return _authorContext.Authors.OrderBy(a => a.LastName).ToList();
        }

        public Author GetAuthor(int authorId)
        {
            return _authorContext.Authors.FirstOrDefault(a => a.Id == authorId);
        }


        public ICollection<Author> GetAuthorsOfABook(int bookId)
        {
            return _authorContext.BookAuthors.Where(b => b.BookId == bookId).Select(a => a.Author).ToList();
        }

        public ICollection<Book> GetBooksByAuthor(int authorId)
        {
            return _authorContext.BookAuthors.Where(a => a.AuthorId == authorId).Select(b => b.Book).ToList();
        }



        public bool AuthorExists(int authorId)
        {
            return _authorContext.Authors.Any(a => a.Id == authorId);
        }
    }
}
