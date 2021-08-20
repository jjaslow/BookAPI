using BookAPIProject.Dtos;
using BookAPIProject.Models;
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
        IBookRepository _bookRepository;

        public CategoriesController(ICategoryRepository categoryRepository, IBookRepository bookRepository)
        {
            _categoryRepository = categoryRepository;
            _bookRepository = bookRepository;
        }

        ////////////////////////////////////////////

        //Uri: api/categories
        [HttpGet]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CategoryDto>))]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
                categoriesDto.Add(new CategoryDto { Id = category.Id, Name = category.Name });

            return Ok(categoriesDto);
        }


        //Uri: api/categories/{categoryId}
        //dont need to manually enter the leading slash. It knows
        [HttpGet("{categoryId}", Name = "GetCategory")]
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
            if (!_bookRepository.BookExists(bookId))
                return NotFound();

            var categories = _categoryRepository.GetCategoriesOfABook(bookId);

            //wont need after we validate author exists
            //if (categories.ToList().Count == 0)
            //    return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoriesDto = new List<CategoryDto>();
            foreach (var category in categories)
                categoriesDto.Add(new CategoryDto { Id = category.Id, Name = category.Name });

            return Ok(categoriesDto);
        }


        //Uri: api/categories/{categoryId}/books
        //dont need to manually enter the leading slash. It knows
        [HttpGet("{categoryId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooksForCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var books = _categoryRepository.GetBooksForCategory(categoryId);

            //wont need after we validate category exists
            //if (books.Count == 0)
            //    return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var booksDto = new List<BookDto>();
            foreach (var book in books)
                booksDto.Add(new BookDto { Id = book.ID, Isbn = book.Isbn, Title = book.Title, DatePublished = book.DatePublished });

            return Ok(booksDto);
        }



        ////////////////////////////////////////////

        //Uri:  //api/categories
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Category))] //Created
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(422)] //unprocessable
        [ProducesResponseType(500)] //Server Error
        public IActionResult CreateCategory([FromBody]Category categoryToCreate)
        {
            //empty POST data
            if (categoryToCreate == null)
                return BadRequest(ModelState);

            //new categoryToCreate Name already exists
            if (_categoryRepository.GetCategories()
                .Any(c => c.Name.Trim().ToUpper() == categoryToCreate.Name.Trim().ToUpper()))
            {
                ModelState.AddModelError("", $"Category {categoryToCreate.Name} already exists");
                return StatusCode(422, ModelState);
            }

            //General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //actually perform the creation and check for errors
            //error saving new country to db (ie SaveChanges int <0)
            if (!_categoryRepository.CreateCategory(categoryToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong saving {categoryToCreate.Name}");
                return StatusCode(500, ModelState);
            }

            //success (response code 201 - created)
            //Then call back to the GetCountry call above to return the newly created country
            //Since GetCategory has an argument, we need to pass that by creating an anonymous object with the same argument name.
            //We also need to add the 'name' of the method to the HttpGet above (so it can be called by name internally). And finally, we pass the newly created object.
            return CreatedAtRoute("GetCategory", new { categoryId = categoryToCreate.Id }, categoryToCreate);
        }



        //URI:  //api/categories/{categoryId}
        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(422)] //unprocessable
        [ProducesResponseType(500)] //Server Error
        public IActionResult UpdateCategory(int categoryId, [FromBody] Category updatedCategoryInfo)
        {
            //empty PUT data
            if (updatedCategoryInfo == null)
                return BadRequest(ModelState);

            ////mis matched Ids
            if (categoryId != updatedCategoryInfo.Id)
                return BadRequest(ModelState);

            ////ensure category ID actually exists
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            ////validate that we arent trying to update this category with a pre-existing name
            if (_categoryRepository.IsDuplicateCategoryName(categoryId, updatedCategoryInfo.Name))
            {
                ModelState.AddModelError("", $"Category {updatedCategoryInfo.Name} already exists");
                return StatusCode(422, ModelState);
            }

            ////General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //error saving new category to db (ie SaveChanges int <0)
            if (!_categoryRepository.UpdateCategory(updatedCategoryInfo))
            {
                ModelState.AddModelError("", $"Something went wrong updating {updatedCategoryInfo.Name}");
                return StatusCode(500, ModelState);
            }

            //success 
            //dont usually return anything on an update
            return NoContent();
        }


        //URI:  //api/categories/{categoryId}
        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)] //no content
        [ProducesResponseType(400)] //bad request
        [ProducesResponseType(404)] //not found
        [ProducesResponseType(409)] //conflict
        [ProducesResponseType(500)] //Server Error
        public IActionResult DeleteCategory(int categoryId)
        {
            ////ensure Category ID actually exists
            if (!_categoryRepository.CategoryExists(categoryId))
                return NotFound();

            var categoryToDelete = _categoryRepository.GetCategory(categoryId);

            //need to check if any books are using this category
            //if so, flag an error
            if (_categoryRepository.GetBooksForCategory(categoryId).Count > 0)
            {
                ModelState.AddModelError("", $"Category {categoryToDelete.Name} cannot be deleted because it is used by at least 1 book");
                return StatusCode(409, ModelState);
            }

            ////General ModelState error
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //error deleting
            if (!_categoryRepository.DeleteCategory(categoryToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {categoryToDelete.Name}");
                return StatusCode(500, ModelState);
            }

            //success 
            //dont usually return anything on an update
            return NoContent();
        }







    }
}
