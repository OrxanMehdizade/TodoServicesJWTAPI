using System.ComponentModel.DataAnnotations;

namespace TodoServicesJWTAPI.Models.DTOs.Product
{
    public class CreateProductRequest
    {
        [Required]
        [MinLength(5)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(5)]
        public string Description { get; set; } = string.Empty;
        [Required]
        [Range(0,int.MaxValue)]
        public int Price { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
