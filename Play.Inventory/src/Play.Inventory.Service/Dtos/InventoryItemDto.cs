using System;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Dtos
{
    public class InventoryItemDto
    {
        public Guid CatalogItemId { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public int Quantity { get; set; }
        public DateTimeOffset AcquiredDate { get; set; }

        public InventoryItemDto() {}
        
        public InventoryItemDto(Guid catalogItemId, string name, string description, int quantity, DateTimeOffset acquiredDate)
        {
            CatalogItemId = catalogItemId;
            Name = name;
            Description = description;
            Quantity = quantity;
            AcquiredDate = acquiredDate;
        }
    }
}