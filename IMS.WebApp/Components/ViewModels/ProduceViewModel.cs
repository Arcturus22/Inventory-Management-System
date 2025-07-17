using IMS.CoreBusiness;
using IMS.WebApp.Components.ViewModelsValidations;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.Components.ViewModels
{
    public class ProduceViewModel
    {
        [Required]
        public string ProductionNumber { get; set; } = string.Empty;
       
        [Range(minimum:1, maximum:int.MaxValue, ErrorMessage ="You need to select a Product")]
        public int  ProductId { get; set; } // def value is 0, which means no inventory selected so not works thus range required
        
        [Range(minimum:1, maximum:int.MaxValue, ErrorMessage ="Quantity has to be greater than 0")]
        [Produce_EnsureEnoughInventoryQuantity] // Custom validation attribute to ensure enough inventory quantity
        public int QuantityToProduce { get; set; }

        public Product? Product { get; set; }

    }
}
