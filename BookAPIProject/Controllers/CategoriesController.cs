using BookAPIProject.Dtos;
using BookAPIProject.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : Controller
    {
        ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }



        //Uri: api/categories
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories().ToList();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
                categoriesDto.Add(new CategoryDto { Id = category.Id, Name = category.Name });

            return Ok(categoriesDto);
        }


        //Uri: api/categories/{categoryId}
        //dont need to manually enter the leading slash. It knows
        [HttpGet("{categoryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(CategoryDto))]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var category = _categoryRepository.GetCategory(categoryId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryDto = new CategoryDto { Id = category.Id, Name = category.Name };

            return Ok(categoryDto);
        }


        //Uri: api/categories/books/{bookId}
        //dont need to manually enter the leading slash. It knows
        [HttpGet("books/{bookId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategoriesOfABook(int bookId)
        {
            //TODO:: validate that book exists

            var categories = _categoryRepository.GetCategoriesOfABook(bookId);

            //wont need after we validate author exists
            if (categories.ToList().Count == 0)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
                categoriesDto.Add(new CategoryDto { Id = category.Id, Name = category.Name });

            return Ok(categoriesDto);
        }


        //Uri: api/categories/books/{categoryId}
        //dont need to manually enter the leading slash. It knows
        [HttpGet("{categoryId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooksForCategory(int categoryId)
        {
            //TODO:: validate that book exists

            var books = _categoryRepository.GetBooksForCategory(categoryId);

            //wont need after we validate author exists
            if (books.Count == 0)
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();
            foreach (var book in books)
                booksDto.Add(new BookDto { Id = book.ID, Isbn = book.Isbn, Title = book.Title, DatePublished = book.DatePublished });

            return Ok(booksDto);
        }
    }
}
