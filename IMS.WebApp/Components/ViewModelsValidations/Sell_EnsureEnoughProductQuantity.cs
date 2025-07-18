using IMS.WebApp.Components.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.Components.ViewModelsValidations
{
    public class Sell_EnsureEnoughProductQuantity : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            var sellViewModel = validationContext.ObjectInstance as SellViewModel;
            if(sellViewModel != null)
            {
                if(sellViewModel.Product !=null && sellViewModel.Product.Quantity < sellViewModel.QuantityToSell)
                {
                    return new ValidationResult($"Not enough Product. Only {sellViewModel.Product.Quantity} products in warehouse.",
                        new[] { validationContext.MemberName });
                }
            }

            return ValidationResult.Success;
        }
    }
}
