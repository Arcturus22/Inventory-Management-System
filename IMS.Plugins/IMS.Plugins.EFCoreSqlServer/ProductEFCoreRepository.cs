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
    public class ProductEFCoreRepository : IProductRepository
    {
        private readonly IDbContextFactory<IMSContext> contextFactory;

        public ProductEFCoreRepository(IDbContextFactory<IMSContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }
        public async Task AddProductsAsync(Product product)
        {
            using var db = contextFactory.CreateDbContext();
            db.Products?.Add(product);
            FlagInventoryUnchanged(product, db);

            await db.SaveChangesAsync();
        }

        public async Task DeleteProductByIdAsync(int productId)
        {
            using var db = contextFactory.CreateDbContext();
            var product = await db.Products.FindAsync(productId);
            if (product is null) return;

            db.Products?.Remove(product);
            await db.SaveChangesAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int prodId)
        {
            using var db = contextFactory.CreateDbContext();
            var product = await db.Products.Include(x => x.ProductInventories)
                .ThenInclude(x=>x.Inventory)
                .FirstOrDefaultAsync(x => x.ProductId == prodId);

            return product ?? null;

        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            using var db = contextFactory.CreateDbContext();
            var products = await db.Products.Where(x => x.ProductName.ToLower().IndexOf(name.ToLower()) >= 0).ToListAsync();

            return products;
        }

        public async Task UpdateProductAsync(Product product)
        {
            using var db = contextFactory.CreateDbContext();

            var prod = await db.Products
                .Include(x => x.ProductInventories)
                .FirstOrDefaultAsync(x=> x.ProductId == product.ProductId);

            if (prod is null) return;

            prod.ProductName = product.ProductName;
            prod.Price = product.Price;
            prod.Quantity = product.Quantity;
            prod.ProductInventories = product.ProductInventories;

            FlagInventoryUnchanged(product, db);
            await db.SaveChangesAsync();
        }


        private void FlagInventoryUnchanged(Product product, IMSContext db)
        {
            if(product?.ProductInventories?.Any() == true)
            {
                foreach(var prodInv in product.ProductInventories)
                {
                    if(prodInv.Inventory is not null)
                    {
                        // trying to connect particular record in the database context
                        db.Entry(prodInv.Inventory).State = EntityState.Unchanged;
                    }
                }
            }
        }

    }
}
