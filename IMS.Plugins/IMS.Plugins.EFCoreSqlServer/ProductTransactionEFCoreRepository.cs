using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class ProductTransactionEFCoreRepository : IProductTransactionRepository
    {
        private readonly IDbContextFactory<IMSContext> contextFactory;
        private readonly IProductRepository productRepository;
        private readonly IInventoryTransactionRepository inventoryTransactionRepository;
        private readonly IInventoryRepository inventoryRepository;

        public ProductTransactionEFCoreRepository(IDbContextFactory<IMSContext> contextFactory,
            IProductRepository productRepository,
            IInventoryTransactionRepository inventoryTransactionRepository,
            IInventoryRepository inventoryRepository)
        {
            this.contextFactory = contextFactory;
            this.productRepository = productRepository;
            this.inventoryTransactionRepository = inventoryTransactionRepository;
            this.inventoryRepository = inventoryRepository;
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneby)
        {
            using var db = contextFactory.CreateDbContext();

            var prod = await productRepository.GetProductByIdAsync(product.ProductId);
            if (prod != null && prod.ProductInventories?.Any() == true)
            {
                foreach (var pi in prod.ProductInventories)
                {
                    if (pi.Inventory != null)
                    {
                        // Add Inventory Transaction Record
                        await inventoryTransactionRepository.ProduceProductAsync(
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
            db.ProductTransactions?.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                DoneBy = doneby,
                QuantityAfter = product.Quantity + quantity,
                TransactionDate = DateTime.Now,
            });

            await db.SaveChangesAsync();
        }

        public async Task SellProductAsync(string salesOrderNumber, Product product, int quantity, string doneBy, double unitPrice)
        {
            using var db = contextFactory.CreateDbContext();

            db.ProductTransactions?.Add(new ProductTransaction
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

            await db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionAsync(string prodName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? transactionType)
        {
            using var db = contextFactory.CreateDbContext();

            var query = from pt in db.ProductTransactions
                        join p in db.Products on pt.ProductId equals p.ProductId
                        where
                            (string.IsNullOrWhiteSpace(prodName) || p.ProductName.ToLower().IndexOf(prodName.ToLower()) >= 0)
                            &&
                            (!dateFrom.HasValue || pt.TransactionDate >= dateFrom.Value.Date) &&
                            (!dateTo.HasValue || pt.TransactionDate <= dateTo.Value.Date) &&
                            (!transactionType.HasValue || pt.ActivityType == transactionType.Value)
                        select pt;

            return await query.Include(x => x.Product).ToListAsync();
        }

    }
}
