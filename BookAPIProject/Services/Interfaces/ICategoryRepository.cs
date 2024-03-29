﻿using BookAPIProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Services
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int categoryId);
        ICollection<Category> GetCategoriesOfABook(int bookId);
        ICollection<Book> GetBooksForCategory(int categoryId);
        bool CategoryExists(int categoryId);

        bool IsDuplicateCategoryName(int categoryId, string categoryName);


        bool CreateCategory(Category newCategory);
        bool UpdateCategory(Category categoryToUpdate);
        bool DeleteCategory(Category categoryToRemove);
        bool Save();

    }
}
