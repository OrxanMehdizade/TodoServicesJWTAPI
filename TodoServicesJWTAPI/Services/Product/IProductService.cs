using TodoServicesJWTAPI.Models.DTOs.Pagintions;
using TodoServicesJWTAPI.Models.DTOs.Product;
using TodoServicesJWTAPI.Models.DTOs.Todo;
using TodoServicesJWTAPI.Models.Entities;
using TodoServicesJWTAPI.Providers;

namespace TodoServicesJWTAPI.Services.Product
{
    public interface IProductService
    {
        Task<ProductItemDto?> GetProduct(int id);
        Task<ProductItemDto> CreateProduct(CreateProductRequest request);
        Task<bool> DeleteProduct(int id);
        Task<PagintionListDto<ProductItemDto>> FilterAll(PagintionRequest request, Category? category, string? productSort, int? minPrice, int? maxPrice);
    }
}
