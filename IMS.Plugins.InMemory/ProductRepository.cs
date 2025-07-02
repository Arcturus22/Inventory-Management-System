using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;

namespace IMS.Plugins.InMemory
{
    public class ProductRepository : IProductRepository
    {
        private List<Product> _products;

        public ProductRepository()
        {
            _products = new List<Product>()
            {
                new Product{ProductId = 1, ProductName = "Bike", Quantity =10 , Price=200},
                new Product{ProductId = 2, ProductName = "Car", Quantity =10 , Price=500},
            };
        }

        public Task AddProductsAsync(Product product)
        {
            if (_products.Any(x => x.ProductName.Equals(product.ProductName, StringComparison.OrdinalIgnoreCase)))
            {
                return Task.CompletedTask; // Invalid product
            }

            var maxId = _products.Any() ? _products.Max(x => x.ProductId) : 0;
            product.ProductId = maxId + 1; // Assign a new Id


            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task DeleteProductByIdAsync(int productId)
        {
            // Won't work for the case when DB is involved because DB doesn't have RemoveAll method.
            //if(productId <= 0)
            //{
            //    return ; // Invalid ID
            //}
            //_products.RemoveAll(x => x.ProductId == productId);

            var invToDelete = _products.FirstOrDefault(x => x.ProductId == productId);
            if (invToDelete is not null)
            {
                _products.Remove(invToDelete);
            }
            return Task.CompletedTask;
        }

        public Task<Product> GetProductByIdAsync(int prodId)
        {
            var product = _products.First(x => x.ProductId == prodId);
            return Task.FromResult(product);
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return await Task.FromResult(_products);

            return await Task.FromResult(_products.Where(x => x.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase)));
        }

        public Task UpdateProductAsync(Product product)
        {
            var invToUpdate = _products.FirstOrDefault(x => x.ProductId == product.ProductId);

            if (invToUpdate is not null)
            {
                invToUpdate.ProductName = product.ProductName;
                invToUpdate.Quantity = product.Quantity;
                invToUpdate.Price = product.Price;
            }

            return Task.CompletedTask;

        }
    }
}
