using AutoMapper;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Mappings
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            CreateMap<ItemDto, Item>().ReverseMap();
        }
    }
}