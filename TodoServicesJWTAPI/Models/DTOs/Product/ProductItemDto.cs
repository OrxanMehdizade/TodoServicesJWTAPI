namespace TodoServicesJWTAPI.Models.DTOs.Product
{
    public class ProductItemDto
    {

        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CategoryId { get; set; }
        public int Price { get; set; }
        public ProductItemDto(string title, string description, int categoryId, int price)
        {
            Title = title;
            Description = description;
            CategoryId = categoryId;
            Price = price;
        }


    }
}
