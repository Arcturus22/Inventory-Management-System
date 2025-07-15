using IMS.CoreBusiness;
using IMS.UseCases.Activities.Interfaces;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.UseCases.Activities
{
    public class PurchaseInventoryUseCase : IPurchaseInventoryUseCase
    {
        private readonly IInventoryRepository inventoryRepository;
        private readonly IInventoryTransactionRepository inventoryTransactionRepository;

        public PurchaseInventoryUseCase(IInventoryRepository inventoryRepository, IInventoryTransactionRepository inventoryTransactionRepository)
        {
            this.inventoryRepository = inventoryRepository;
            this.inventoryTransactionRepository = inventoryTransactionRepository;
        }
        public async Task ExecuteAsync(string poNumber, Inventory inventory, int quantity, string doneBy)
        {
            // Insert a record in the transaction table
            inventoryTransactionRepository.PurchaseAsync(poNumber, inventory, quantity, doneBy, inventory.Price);

            // Update the inventory table with the new quantity
            inventory.Quantity += quantity;
            await this.inventoryRepository.UpdateInventoryAsync(inventory);

        }
    }
}
