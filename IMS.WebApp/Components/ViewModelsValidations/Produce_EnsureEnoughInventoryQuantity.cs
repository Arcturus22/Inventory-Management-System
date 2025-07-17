using IMS.WebApp.Components.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.Components.ViewModelsValidations
{
    public class Produce_EnsureEnoughInventoryQuantity : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            var produceViewModel = validationContext.ObjectInstance as ProduceViewModel;
            if(produceViewModel != null)
            {
                if(produceViewModel.Product != null && produceViewModel.Product.ProductInventories != null)
                {
                    // Validating now coz ensured that Product and ProductInventories are not null
                    foreach(var pi in produceViewModel.Product.ProductInventories)
                    {
                        if(pi.Inventory!=null && (pi.InventoryQuantity * produceViewModel.QuantityToProduce) > pi.Inventory.Quantity)
                        {
                            return new ValidationResult($"The inventory ({pi.Inventory.InventoryName}) is not enough to produce {produceViewModel.QuantityToProduce} products",
                                new[] {validationContext.MemberName});
                        }
                    }

                }

            }
            return ValidationResult.Success; // If all checks passed, return success
        }
    }
}
