using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Client.Services.CategoryService
{
    public interface ICategoryService
    {
        List<Category> Categories { get; set; }
        Task GetCategories();
    }
}