using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category> GetByIdAsync(Guid id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(Guid id);
        Task<bool> IsDeletedAsync(Guid id);
    }
}
