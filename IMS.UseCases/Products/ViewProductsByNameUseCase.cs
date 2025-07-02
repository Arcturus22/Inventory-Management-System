using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using IMS.UseCases.Products.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Products
{
    // Changed from internal to public to allow access from other projects  
    public class ViewProductsByNameUseCase : IViewProductsByNameUseCase
    {
        private readonly IProductRepository _productRepository;

        public ViewProductsByNameUseCase(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }   
        public async Task<IEnumerable<Product>> ExecuteAsync(string name = "")
        {
            return await _productRepository.GetProductsByNameAsync(name);
        }
    }
}
