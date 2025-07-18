using IMS.CoreBusiness;
using IMS.WebApp.Components.ViewModelsValidations;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.Components.ViewModels
{
    public class SellViewModel
    {
        [Required]
        public string SalesOrderNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "You need to select a Product")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 1")]
        [Sell_EnsureEnoughProductQuantity]
        public int QuantityToSell { get; set; }


        [Range(0, int.MaxValue, ErrorMessage = "Price can't be negative")]
        public double UnitPrice { get; set; }
        public Product? Product { get; set; } = null;
    }
}
