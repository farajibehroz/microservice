using MassTransit;
using Play.Catalog.Contracts;
using Play.Common.Repositories;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemCreatedConsumers : IConsumer<CatalogCreateItemDto>
    {
        private readonly IRepository<CatalogItem> _catalogItemRepository;

        public CatalogItemCreatedConsumers(IRepository<CatalogItem> catalogItemRepository)
        {
            _catalogItemRepository = catalogItemRepository;
        }

        public async Task Consume(ConsumeContext<CatalogCreateItemDto> context)
        {
            var message = context.Message;
            var item = await _catalogItemRepository.GetAsync(message.Id);
            if (item != null)
            {
                return;
            }
            var catalogItem = new CatalogItem()
            {
                Id = message.Id,
                Name = message.Name,
                Description = message.Description
            };
            await _catalogItemRepository.CreateAsync(catalogItem);
        }
    }

    public class CatalogItemUpdatedConsumers : IConsumer<CatalogUpdateItemDto>
    {
        private readonly IRepository<CatalogItem> _catalogItemRepository;

        public CatalogItemUpdatedConsumers(IRepository<CatalogItem> catalogItemRepository)
        {
            _catalogItemRepository = catalogItemRepository;
        }

        public async Task Consume(ConsumeContext<CatalogUpdateItemDto> context)
        {
            var message = context.Message;
            var item = await _catalogItemRepository.GetAsync(message.Id);
            if (item == null)
            {
                return;
            }

            item.Name = message.Name;
            item.Description = message.Description;
           
            await _catalogItemRepository.UpdateAsync(item);
        }
    }

    public class CatalogItemDeletedConsumers : IConsumer<CatalogDeleteItemDto>
    {
        private readonly IRepository<CatalogItem> _catalogItemRepository;

        public CatalogItemDeletedConsumers(IRepository<CatalogItem> catalogItemRepository)
        {
            _catalogItemRepository = catalogItemRepository;
        }

        public async Task Consume(ConsumeContext<CatalogDeleteItemDto> context)
        {
            var message = context.Message;
            var item = await _catalogItemRepository.GetAsync(message.Id);
            if (item == null)
            {
                return;
            }
            await _catalogItemRepository.DeleteAsync(item.Id);
        }
    }

}