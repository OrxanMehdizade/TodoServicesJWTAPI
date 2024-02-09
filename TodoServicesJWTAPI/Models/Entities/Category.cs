namespace TodoServicesJWTAPI.Models.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<ProductItem>? Products { get; set; }
    }
}
