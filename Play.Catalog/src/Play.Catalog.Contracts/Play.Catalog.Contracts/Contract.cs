
namespace Play.Catalog.Contracts
{
    public record CatalogCreateItemDto(Guid Id, string Name, string Description);
    public record CatalogUpdateItemDto(Guid Id, string Name, string Description);

    public record CatalogDeleteItemDto(Guid Id);
}