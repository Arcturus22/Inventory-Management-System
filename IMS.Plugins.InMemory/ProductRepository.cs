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

        public Task<Product?> GetProductByIdAsync(int prodId)
        {
            var prod = _products.First(x => x.ProductId == prodId);

            var newProd = new Product();
            if (prod is not null)
            {
                newProd.ProductId = prod.ProductId;
                newProd.ProductName = prod.ProductName;
                newProd.Quantity = prod.Quantity;
                newProd.Price = prod.Price;
                newProd.ProductInventories = new List<ProductInventory>();

                if (prod.ProductInventories?.Any() == true)
                {
                    foreach (var prodInv in prod.ProductInventories)
                    {

                        var newProdInv = new ProductInventory
                        {
                            InventoryId = prodInv.InventoryId,
                            ProductId = prodInv.ProductId,
                            Product = prod,
                            Inventory = new Inventory(),
                            InventoryQuantity = prodInv.InventoryQuantity
                        };

                        if (prodInv.Inventory is not null)
                        {
                            newProdInv.Inventory.InventoryId = prodInv.Inventory.InventoryId;
                            newProdInv.Inventory.InventoryName = prodInv.Inventory.InventoryName;
                            newProdInv.Inventory.Quantity = prodInv.Inventory.Quantity;
                            newProdInv.Inventory.Price = prodInv.Inventory.Price;
                        }

                        newProd.ProductInventories.Add(newProdInv);
                    }
                }

            }
            return Task.FromResult(newProd);
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return await Task.FromResult(_products);

            return await Task.FromResult(_products.Where(x => x.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase)));
        }

        public Task UpdateProductAsync(Product product)
        {
            // Prevent different prodct with same name
            if (_products.Any(x => x.ProductId != product.ProductId &&
                                x.ProductName.ToLower() == product.ProductName.ToLower())
                )
                return Task.CompletedTask;

            var prodToUpdate =  _products.FirstOrDefault(x => x.ProductId == product.ProductId);

            if (prodToUpdate is not null)
            {
                prodToUpdate.ProductName = product.ProductName;
                prodToUpdate.Quantity = product.Quantity;
                prodToUpdate.Price = product.Price;
                prodToUpdate.ProductInventories = prodToUpdate.ProductInventories;
            }

            return Task.CompletedTask;

        }
    }
}
