using AutoMapper;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Mappings
{
    public class CatalogItemProfile : Profile
    {
        public CatalogItemProfile()
        {
            CreateMap<CatalogItem, InventoryItemDto>(MemberList.Source)
                .ForMember(dest => dest.CatalogItemId, opt => opt.MapFrom(src => src.Id));
        }
    }
}