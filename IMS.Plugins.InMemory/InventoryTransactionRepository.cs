﻿using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Plugins.InMemory
{
    public class InventoryTransactionRepository : IInventoryTransactionRepository
    {
        private readonly IInventoryRepository inventoryRepository;
        public List<InventoryTransaction> _inventoryTransactions = new List<InventoryTransaction>();

        public InventoryTransactionRepository(IInventoryRepository inventoryRepository)
        {
            this.inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactionAsync(string inventoryName, DateTime? dateFrom, DateTime? dateTo, InventoryTransactionType? transactionType)
        {
            var inventories = (await inventoryRepository.GetInventoriesByNameAsync(string.Empty)).ToList();

            var query = from it in _inventoryTransactions
                        join inv in inventories on it.InventoryId equals inv.InventoryId
                        where 
                            (string.IsNullOrWhiteSpace(inventoryName) || inv.InventoryName.ToLower().IndexOf(inventoryName.ToLower()) >= 0 )
                            && 
                            (!dateFrom.HasValue || it.TransactionDate >= dateFrom.Value.Date) &&
                            (!dateTo.HasValue || it.TransactionDate <= dateTo.Value.Date) &&
                            (!transactionType.HasValue || it.ActivityType == transactionType.Value)
                        select new InventoryTransaction
                        {
                            Inventory = inv,
                            InventoryTransactionId = it.InventoryTransactionId,
                            poNumber = it.poNumber,
                            InventoryId = it.InventoryId,
                            QuantityBefore = it.QuantityBefore,
                            QuantityAfter = it.QuantityAfter,
                            ActivityType = it.ActivityType,
                            TransactionDate = it.TransactionDate,
                            DoneBy = it.DoneBy,
                            UnitPrice = it.UnitPrice
                        };

            return query;
        }

        public void ProduceProductAsync(string productionNumber, Inventory inventory, int quantityToConsume, string doneBy, double price)
        {
            this._inventoryTransactions.Add(new InventoryTransaction
            {
                ProductionNumber = productionNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.ProduceProduct,
                QuantityAfter = inventory.Quantity - quantityToConsume,
                UnitPrice = price,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
            });
        }

        public void PurchaseAsync(string poNumber, Inventory inventory, int quantity, string doneBy, double price)
        {
            this._inventoryTransactions.Add(new InventoryTransaction
            {
                poNumber = poNumber,
                InventoryId = inventory.InventoryId,
                QuantityBefore = inventory.Quantity,
                ActivityType = InventoryTransactionType.PurchaseInventory,
                QuantityAfter = inventory.Quantity + quantity,
                UnitPrice = price,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
            });
        }
    }
}
