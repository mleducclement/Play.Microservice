using System;

namespace Play.Inventory.Service.Dtos
{
    public class GrantItemDto
    {
        public Guid UserId { get; init; }
        public Guid CatalogItemId { get; init; }
        public int Quantity { get; init; }
        
        public GrantItemDto() {}
        
        public GrantItemDto(Guid userId, Guid catalogItemId, int quantity)
        {
            UserId = userId;
            CatalogItemId = catalogItemId;
            Quantity = quantity;
        }
    }
}