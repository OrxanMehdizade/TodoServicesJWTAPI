namespace TodoServicesJWTAPI.Models.Entities
{
    public class ProductItem : BaseEntity
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Price { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }

    }
}
