using IMS.CoreBusiness.Validations;
using System.ComponentModel.DataAnnotations;

namespace IMS.CoreBusiness
{
    public class Product
    {
        public int ProductId { get; set; }
        

        [Required(ErrorMessage = "The Product Name is required.")]
        [StringLength(150, ErrorMessage = "The Product Name cannot exceed length 150.")]
        public string ProductName { get; set; } = string.Empty;
        

        [Range(0,int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 0.")]
        public int Quantity { get; set; }


        [Range(0,int.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
        public double Price { get; set; }

        // Navigation property for the association with ProductInventory
        [Product_EnsurePriceIsGreaterThanInventoriesCost]
        public List<ProductInventory> ProductInventories { get; set; } = new List<ProductInventory>();


        // Helper function to add a inventory into the productiventory list
        public void AddInventory(Inventory inventory)
        {
            if(!this.ProductInventories.Any
                (pi => pi.Inventory is not null &&
            pi.Inventory.InventoryName.Equals(inventory.InventoryName)))
            {

            this.ProductInventories.Add(new ProductInventory{
                InventoryId = inventory.InventoryId,
                    Inventory = inventory,
                    InventoryQuantity = 1,
                    ProductId = this.ProductId, 
                    Product = this
            });
            }
        }
        public void RemoveInventory(ProductInventory productInventory)
        {
            ProductInventories?.Remove(productInventory);
        }


    }
}
