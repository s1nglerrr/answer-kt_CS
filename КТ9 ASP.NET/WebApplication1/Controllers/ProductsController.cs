using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductsController(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            if (await _categoryRepository.IsDeletedAsync(dto.CategoryId))
                return BadRequest("Категория не найдена или удалена");

            Product product = new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                IsDeleted = false
            };

            await _productRepository.AddAsync(product);
            return CreatedAtAction(nameof(GetAll), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            if (await _productRepository.IsDeletedAsync(id))
                return NotFound("Товар не найден или уже удален");

            Product existingProduct = await _productRepository.GetByIdAsync(id);

            existingProduct.Name = dto.Name ?? existingProduct.Name;
            existingProduct.Price = dto.Price ?? existingProduct.Price;
            existingProduct.Description = dto.Description ?? existingProduct.Description;
            existingProduct.CategoryId = dto.CategoryId ?? existingProduct.CategoryId;

            try
            {
                await _productRepository.UpdateAsync(existingProduct);
                return Ok(existingProduct);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _productRepository.IsDeletedAsync(id))
                return NotFound("Товар не найден или уже удален");

            await _productRepository.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Product> products = await _productRepository.GetAllAsync();
            return Ok(products);
        }
    }
}
