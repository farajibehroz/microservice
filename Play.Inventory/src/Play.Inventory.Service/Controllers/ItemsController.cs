using Microsoft.AspNetCore.Mvc;
using Play.Common.Repositories;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Dtos;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Controllers
{

    [ApiController]
    [Route("Items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> _inventoryItemRepository;
        private readonly IRepository<CatalogItem> _catalogItemRepository;
        // private readonly CatalogClient _catalogClient;
        public ItemsController(IRepository<InventoryItem> inventoryItemRepository,
            IRepository<CatalogItem> catalogItemRepository)
        {
            _inventoryItemRepository = inventoryItemRepository;
            _catalogItemRepository = catalogItemRepository;
        }

        [HttpGet]
        [Route("{userId:Guid}")]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }
            var catalogItems = await _catalogItemRepository.GetAllAsync();
            var items = await _inventoryItemRepository.GetAllAsync(x => x.UserId == userId);

            var inventoryItems = items.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(x => x.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });
            return Ok(inventoryItems);

        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemDto grantItemDto)
        {
            var inventoryItem = await _inventoryItemRepository.GetAsync(x => x.UserId == grantItemDto.UserId && x.CatalogItemId == grantItemDto.CatalogItemId);
            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem()
                {
                    UserId = grantItemDto.UserId,
                    CatalogItemId = grantItemDto.CatalogItemId,
                    Quantity = grantItemDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await _inventoryItemRepository.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity = +grantItemDto.Quantity;
                await _inventoryItemRepository.UpdateAsync(inventoryItem);
            }

            return CreatedAtAction(nameof(GetAsync), new { userId = inventoryItem.UserId }, inventoryItem);
        }

    }
}