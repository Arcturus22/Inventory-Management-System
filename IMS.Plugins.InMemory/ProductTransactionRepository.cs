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
                            0);

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

        public Task SellProductAsync(string salesOrderNumber, Product product, int quantity, string doneBy, double unitPrice)
        {
            _productTransactions.Add(new ProductTransaction
            {
                SONumber = salesOrderNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                QuantityAfter = product.Quantity - quantity,
                ActivityType = ProductTransactionType.SellProduct,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = unitPrice
            });

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionAsync(string prodName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? transactionType)
        {
            var products = (await productRepository.GetProductsByNameAsync(string.Empty)).ToList();

            var query = from pt in _productTransactions
                        join p in products on pt.ProductId equals p.ProductId
                        where
                            (string.IsNullOrWhiteSpace(prodName) || p.ProductName.ToLower().IndexOf(prodName.ToLower()) >= 0)
                            &&
                            (!dateFrom.HasValue || pt.TransactionDate >= dateFrom.Value.Date) &&
                            (!dateTo.HasValue || pt.TransactionDate <= dateTo.Value.Date) &&
                            (!transactionType.HasValue || pt.ActivityType == transactionType.Value)
                        select new ProductTransaction
                        {
                            Product = p,
                            ProductTransactionId = pt.ProductTransactionId,
                            ProductionNumber = pt.ProductionNumber,
                            SONumber = pt.SONumber,
                            ProductId = pt.ProductId,
                            QuantityBefore = pt.QuantityBefore,
                            QuantityAfter = pt.QuantityAfter,
                            ActivityType = pt.ActivityType,
                            TransactionDate = pt.TransactionDate,
                            DoneBy = pt.DoneBy,
                            UnitPrice = pt.UnitPrice
                        };

            return query;
        }

    }
}
