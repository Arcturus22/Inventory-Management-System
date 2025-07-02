using IMS.CoreBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.PluginInterfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProductsByNameAsync(string name);
        Task DeleteProductByIdAsync(int productId);
        Task AddProductsAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task<Product> GetProductByIdAsync(int prodId);
    }
}
