
using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service.Dtos;
public record ItemDto(Guid Id, [Required] string Name, string Description, [Range(1, 100)] decimal Price, DateTimeOffset CreateDateTime);
public record CreateItemDto([Required] string Name, string Description, [Range(1, 100)] decimal Price);
public record UpdateItemDto([Required] string Name, string Description, [Range(1, 100)] decimal Price);