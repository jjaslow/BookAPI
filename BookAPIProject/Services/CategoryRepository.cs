using BookAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        BookDbContext _categoryContext;

        public CategoryRepository(BookDbContext bookContext)
        {
            _categoryContext = bookContext;
        }



        public ICollection<Category> GetCategories()
        {
            return _categoryContext.Categories.OrderBy(c => c.Name).ToList();
        }

        public Category GetCategory(int categoryId)
        {
            return _categoryContext.Categories.FirstOrDefault(c => c.Id == categoryId);
        }

        public ICollection<Category> GetCategoriesOfABook(int bookId)
        {
            return _categoryContext.BookCategories.Where(b => b.BookId == bookId).Select(c=>c.Category).ToList();
        }

        public ICollection<Book> GetBooksForCategory(int categoryId)
        {
            return _categoryContext.BookCategories.Where(c => c.CategoryId == categoryId).Select(b => b.Book).ToList();
        }

        public bool CategoryExists(int categoryId)
        {
            return _categoryContext.Categories.Any(c => c.Id == categoryId);
        }
    }
}
