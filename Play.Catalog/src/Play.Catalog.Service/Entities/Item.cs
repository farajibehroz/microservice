using Play.Common;

namespace Play.Catalog.Service.Entities
{
    public class Item : IEntity
    {
        public Item()
        {

        }
        public Item(Guid id, string name, string description, decimal price, DateTimeOffset createDateTime)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            CreateDateTime = createDateTime;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public DateTimeOffset CreateDateTime { get; set; }
    }
}
