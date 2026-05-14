using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            Category category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                IsDeleted = false
            };

            await _categoryRepository.AddAsync(category);
            return CreatedAtAction(nameof(GetAll), new { id = category.Id }, category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _categoryRepository.IsDeletedAsync(id))
                return NotFound("Категория не найдена или удалена");

            await _categoryRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Category> categories = await _categoryRepository.GetAllAsync();
            return Ok(categories);
        }
    }
}
