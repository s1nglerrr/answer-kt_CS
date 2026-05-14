using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EventStoreService _eventStore;
        private readonly AppDbContext _dbContext;

        public ProductRepository(EventStoreService eventStore, AppDbContext dbContext)
        {
            _eventStore = eventStore;
            _dbContext = dbContext;
        }

        private string GetStreamName(Guid id) => $"product-{id}";

        public async Task<Product> GetByIdAsync(Guid id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products
                .Where(p => !p.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _eventStore.AppendEventAsync(
                GetStreamName(product.Id),
                new ProductCreated
                {
                    Id = product.Id,
                    CategoryId = product.CategoryId,
                    Name = product.Name,
                    Price = product.Price,
                    Description = product.Description
                }
            );

            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            Product? existingProduct = await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == product.Id);

            ProductUpdated updatedEvent = new ProductUpdated { Id = product.Id };
            bool hasChanges = false;

            if (existingProduct.Name != product.Name)
            {
                updatedEvent.Name = product.Name;
                hasChanges = true;
            }

            if (existingProduct.Price != product.Price)
            {
                updatedEvent.Price = product.Price;
                hasChanges = true;
            }

            if (existingProduct.Description != product.Description)
            {
                updatedEvent.Description = product.Description;
                hasChanges = true;
            }

            if (existingProduct.CategoryId != product.CategoryId)
            {
                updatedEvent.CategoryId = product.CategoryId;
                hasChanges = true;
            }

            if (!hasChanges)
                throw new InvalidOperationException("Изменений не обнаружено");

            await _eventStore.AppendEventAsync(
                GetStreamName(product.Id),
                updatedEvent
            );

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _eventStore.AppendEventAsync(
                GetStreamName(id),
                new ProductDeleted { Id = id }
            );

            Product? product = await _dbContext.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                _dbContext.Products.Update(product);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> IsDeletedAsync(Guid id)
        {
            Product? product = await _dbContext.Products.FindAsync(id);
            return product?.IsDeleted ?? true;
        }
    }
}
