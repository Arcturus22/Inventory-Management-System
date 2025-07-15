using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.Components.ViewModels
{
    public class PurchaseViewModel
    {
        [Required]
        public string poNumber { get; set; } = string.Empty;
        [Range(minimum:1, maximum:int.MaxValue, ErrorMessage ="You need to select an inventory")]
        public int  InventoryId { get; set; } // def value is 0, which means no inventory selected so not works thus range required
        [Range(minimum:1, maximum:int.MaxValue, ErrorMessage ="Quantity has to be greater than 0")]
        public int QuantityToPurchase { get; set; }
        public double InventoryPrice { get; set; }

    }
}
