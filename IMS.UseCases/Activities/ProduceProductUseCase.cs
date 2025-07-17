using IMS.CoreBusiness;
using IMS.UseCases.Activities.Interfaces;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Activities
{
    public class ProduceProductUseCase : IProduceProductUseCase
    {
        private readonly IProductTransactionRepository productTransactionRepository;
        private readonly IProductRepository productRepository;

        public ProduceProductUseCase(IProductTransactionRepository productTransactionRepository,
            IProductRepository productRepository)
        {
            this.productTransactionRepository = productTransactionRepository;
            this.productRepository = productRepository;
        }
        public async Task ExecuteAsync(string productionNumber, Product product, int quantity, string doneby)
        {
            // Add Transaction Record
            // Decrease the product inventory quantity
            await productTransactionRepository.ProduceAsync(productionNumber, product, quantity, doneby );

            // Update the product inventory quantity
            product.Quantity += quantity;
            await productRepository.UpdateProductAsync(product);

        }
    }
}
