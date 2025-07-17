using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.InMemory
{
    public class ProductTransactionRepository : IProductTransactionRepository
    {
        private List<ProductTransaction> _productTransactions = new List<ProductTransaction>();

        private readonly IProductRepository productRepository;
        private readonly IInventoryTransactionRepository inventoryTransactionRepository;
        private readonly IInventoryRepository inventoryRepository;

        public ProductTransactionRepository(IProductRepository productRepository,
            IInventoryTransactionRepository inventoryTransactionRepository,
            IInventoryRepository inventoryRepository)
        {
            this.productRepository = productRepository;
            this.inventoryTransactionRepository = inventoryTransactionRepository;
            this.inventoryRepository = inventoryRepository;
        }
        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneby)
        {
            var prod = await productRepository.GetProductByIdAsync(product.ProductId);
            if (prod != null)
            {
                foreach (var pi in prod.ProductInventories)
                {
                    if (pi.Inventory != null)
                    {
                        // Add Inventory Transaction Record
                        inventoryTransactionRepository.ProduceProductAsync(
                            productionNumber,
                            pi.Inventory,
                            pi.InventoryQuantity * quantity,
                            doneby,
                            -1);

            // Decrease Inventories
                        var inv = await inventoryRepository.GetInventoryByIdAsync(pi.InventoryId);
                        inv.Quantity -= pi.InventoryQuantity * quantity;
                        await inventoryRepository.UpdateInventoryAsync(inv);
                    }
                }
            }

            // Add Product Transaction Record
            _productTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                DoneBy = doneby,
                QuantityAfter = product.Quantity + quantity,
                TransactionDate = DateTime.Now,
            });

        }
    }
}
