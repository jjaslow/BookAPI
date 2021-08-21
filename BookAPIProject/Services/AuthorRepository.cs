using BookAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    public class AuthorRepository : IAuthorRepositiory
    {
        BookDbContext _context;

        public AuthorRepository(BookDbContext context)
        {
            _context = context;
        }


        ///////////////////////////////////


        public ICollection<Author> GetAuthors()
        {
            return _context.Authors.OrderBy(a => a.LastName).ToList();
        }

        public Author GetAuthor(int authorId)
        {
            return _context.Authors.FirstOrDefault(a => a.Id == authorId);
        }


        public ICollection<Author> GetAuthorsOfABook(int bookId)
        {
            return _context.BookAuthors.Where(b => b.BookId == bookId).Select(a => a.Author).ToList();
        }

        public ICollection<Book> GetBooksByAuthor(int authorId)
        {
            return _context.BookAuthors.Where(a => a.AuthorId == authorId).Select(b => b.Book).ToList();
        }



        public bool AuthorExists(int authorId)
        {
            return _context.Authors.Any(a => a.Id == authorId);
        }


        ///////////////////////////////////




        public bool CreateAuthor(Author author)
        {
            _context.Add(author);
            return Save();
        }

        public bool UpdateAuthor(Author author)
        {
            _context.Update(author);
            return Save();
        }

        public bool DeleteAuthor(Author author)
        {
            _context.Remove(author);
            return Save();
        }

        public bool Save()
        {
            int result = _context.SaveChanges();
            return result >= 0;
        }





    }
}
