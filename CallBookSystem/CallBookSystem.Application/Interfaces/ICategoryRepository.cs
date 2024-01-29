using CallBookSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallBookSystem.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetAllCategoriesAsync();
        Task<IReadOnlyList<Category>> GetAllCategoriesByProcessIdAsync(string processId);
        Task<Category> GetCategoryAsync(string id, string categoryName, string processId, string actionType);
        Task<int> SaveCategoryAsync(Category category, string userId, string actionType);
    }
}
