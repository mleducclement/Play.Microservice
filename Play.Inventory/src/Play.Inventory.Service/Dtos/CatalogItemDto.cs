using System;

namespace Play.Inventory.Service.Dtos
{
    public class CatalogItemDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        
        public CatalogItemDto () {}
        
        public CatalogItemDto(Guid id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}