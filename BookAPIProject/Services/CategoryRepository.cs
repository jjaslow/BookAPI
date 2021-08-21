using BookAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        BookDbContext _context;

        public CategoryRepository(BookDbContext bookContext)
        {
            _context = bookContext;
        }

        //////////////


        public ICollection<Category> GetCategories()
        {
            return _context.Categories.OrderBy(c => c.Name).ToList();
        }

        public Category GetCategory(int categoryId)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == categoryId);
        }

        public ICollection<Category> GetCategoriesOfABook(int bookId)
        {
            return _context.BookCategories.Where(b => b.BookId == bookId).Select(c=>c.Category).ToList();
        }

        public ICollection<Book> GetBooksForCategory(int categoryId)
        {
            return _context.BookCategories.Where(c => c.CategoryId == categoryId).Select(b => b.Book).ToList();
        }

        public bool CategoryExists(int categoryId)
        {
            return _context.Categories.Any(c => c.Id == categoryId);
        }

        public bool IsDuplicateCategoryName(int categoryId, string categoryName)
        {
            bool categoryNameExists = _context.Categories.Any(c => c.Name == categoryName);
            if (!categoryNameExists)
                return false;
            else
            {
                Category existingCategory = _context.Categories.FirstOrDefault(c => c.Name.Trim().ToUpper() == categoryName.Trim().ToUpper());
                return existingCategory.Id != categoryId;
            }
        }




        //////////////


        public bool CreateCategory(Category newCategory)
        {
            _context.Add(newCategory);
            return Save();
        }

        public bool UpdateCategory(Category categoryToUpdate)
        {
            _context.Update(categoryToUpdate);
            return Save();
        }

        public bool DeleteCategory(Category categoryToRemove)
        {
            _context.Remove(categoryToRemove);
            return Save();
        }

        public bool Save()
        {
            int response = _context.SaveChanges();
            return response >= 0;
        }

    }
}
