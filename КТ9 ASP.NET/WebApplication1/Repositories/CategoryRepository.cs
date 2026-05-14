using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EventStoreService _eventStore;
        private readonly AppDbContext _dbContext;

        public CategoryRepository(EventStoreService eventStore, AppDbContext dbContext)
        {
            _eventStore = eventStore;
            _dbContext = dbContext;
        }

        private string GetStreamName(Guid id) => $"category-{id}";

        public async Task<Category> GetByIdAsync(Guid id)
        {
            return await _dbContext.Categories.FindAsync(id);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _dbContext.Categories
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(Category category)
        {
            await _eventStore.AppendEventAsync(
                GetStreamName(category.Id),
                new CategoryCreated
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description
                }
            );

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            Category? existingCategory = await _dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == category.Id);

            CategoryUpdated updatedEvent = new CategoryUpdated { Id = category.Id };
            bool hasChanges = false;

            if (existingCategory.Name != category.Name)
            {
                updatedEvent.Name = category.Name;
                hasChanges = true;
            }

            if (existingCategory.Description != category.Description)
            {
                updatedEvent.Description = category.Description;
                hasChanges = true;
            }

            if (!hasChanges)
                throw new InvalidOperationException("Изменений не обнаружено");

            await _eventStore.AppendEventAsync(
                GetStreamName(category.Id),
                updatedEvent
            );

            _dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _eventStore.AppendEventAsync(
                GetStreamName(id),
                new CategoryDeleted { Id = id }
            );

            Category? category = await _dbContext.Categories.FindAsync(id);
            if (category != null)
            {
                category.IsDeleted = true;
                _dbContext.Categories.Update(category);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> IsDeletedAsync(Guid id)
        {
            Category? category = await _dbContext.Categories.FindAsync(id);
            return category?.IsDeleted ?? true;
        }
    }
}
