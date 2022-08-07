using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Common.Repositories;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("Items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> _itemsRepository;
        private readonly IPublishEndpoint _publishEndpoint;


        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            _itemsRepository = itemsRepository;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItemsAsync()
        {
            var items = (await _itemsRepository.GetAllAsync()).Select(x => x.AsItemDto());
            return Ok(items);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync(Guid id)
        {
            var item = await _itemsRepository.GetAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item.AsItemDto());
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItem(CreateItemDto itemDto)
        {
            var newItem = new Item(Guid.NewGuid(), itemDto.Name, itemDto.Description, itemDto.Price, DateTimeOffset.UtcNow);

            await _itemsRepository.CreateAsync(newItem);
            await _publishEndpoint.Publish(new CatalogCreateItemDto(newItem.Id, newItem.Name, newItem.Description));

            return CreatedAtAction(nameof(GetItemAsync), new { id = newItem.Id }, newItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItemAsync(Guid id, UpdateItemDto itemDto)
        {
            var item = await _itemsRepository.GetAsync(id);
            if (item == null)
                return NotFound();

            item.Name = itemDto.Name;
            item.Description = itemDto.Description;
            item.Price = itemDto.Price;

            await _itemsRepository.UpdateAsync(item);
            await _publishEndpoint.Publish(new CatalogUpdateItemDto(item.Id, item.Name, item.Description));
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = await _itemsRepository.GetAsync(id);
            if (item == null)
                return NotFound();

            await _itemsRepository.DeleteAsync(item.Id);
            await _publishEndpoint.Publish(new CatalogDeleteItemDto(item.Id));

            return NoContent();
        }
    }
}