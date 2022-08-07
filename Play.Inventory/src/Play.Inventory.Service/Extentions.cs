using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service
{
    public static class Extentions
    {
        public static InventoryItemDto AsDto(this InventoryItem inventoryItem, string name, string description)
        {
            return new InventoryItemDto(inventoryItem.Id,name,description ,inventoryItem.Quantity, inventoryItem.AcquiredDate);
        }
    }
}