using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service
{
    public static class Extentions
    {

        public static ItemDto AsItemDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreateDateTime);
        }

        public static Item AsItem(this ItemDto item)
        {
            return new Item(item.Id, item.Name, item.Description, item.Price, item.CreateDateTime);
        }

    }
}
