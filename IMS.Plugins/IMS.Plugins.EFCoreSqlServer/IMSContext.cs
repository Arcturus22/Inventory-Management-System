using IMS.CoreBusiness;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer
{
    // This class represents the database itself
    // The classes in IMS.CoreBusiness represents the tables in the database
    public class IMSContext : DbContext
    {
        public IMSContext(DbContextOptions<IMSContext> options) : base(options)
        {
            
        }


        // Tell Entity Framework Core about the tables in the database
        // Basically telling what tables are in it

        public DbSet<Inventory>? Inventories { get; set; }
        public DbSet<Product>? Products { get; set; }

        // Many to many relationship between Product and Inventory
        // Mapping Object
        public DbSet<ProductInventory>? ProductInventories { get; set; }

        public DbSet<InventoryTransaction>? InventoryTransactions { get; set; }
        public DbSet<ProductTransaction>? ProductTransactions { get; set; }


        // How to specify the relationships between the tables
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // When the model is being created , we can specify the relationships
            // This is where we configure the many to many relationship between Product and Inventory
            // Models = Tables in the database -> Model BUilder

            modelBuilder.Entity<ProductInventory>()
                .HasKey(pi => new { pi.ProductId, pi.InventoryId }); // Composite Key

            modelBuilder.Entity<ProductInventory>()
                .HasOne(pi => pi.Product)
                .WithMany(pi => pi.ProductInventories)
                .HasForeignKey(pi => pi.ProductId);

            modelBuilder.Entity<ProductInventory>()
                .HasOne(pi => pi.Inventory)
                .WithMany(i => i.ProductInventories)
                .HasForeignKey(pi => pi.InventoryId);

            // Seeding data
            modelBuilder.Entity<Inventory>().HasData(

                new Inventory { InventoryId = 1, InventoryName = "Bike Seat", Quantity = 10, Price = 2 },
                new Inventory { InventoryId = 2, InventoryName = "Bike Body", Quantity = 10, Price = 15 },
                new Inventory { InventoryId = 3, InventoryName = "Bike Wheels", Quantity = 20, Price = 8 },
                new Inventory { InventoryId = 4, InventoryName = "Bike Pedals", Quantity = 20, Price = 1 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, ProductName = "Bike", Quantity = 10, Price = 200 },
                new Product { ProductId = 2, ProductName = "Car", Quantity = 10, Price = 500 }
            );

            modelBuilder.Entity<ProductInventory>().HasData(
                new ProductInventory { ProductId = 1, InventoryId = 1, InventoryQuantity = 1 }, // Bike Seat
                new ProductInventory { ProductId = 1, InventoryId = 2, InventoryQuantity = 1 }, // Bike Body
                new ProductInventory { ProductId = 1, InventoryId = 3, InventoryQuantity = 2 }, // Bike Wheels
                new ProductInventory { ProductId = 1, InventoryId = 4, InventoryQuantity = 2 }  // Bike Pedals
            );

        }



    }
}
