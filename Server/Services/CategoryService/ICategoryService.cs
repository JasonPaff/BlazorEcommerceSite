using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Server.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<ServiceResponse<List<Category>>> GetCategories();
    }
}