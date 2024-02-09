using Microsoft.EntityFrameworkCore;
using System.Linq;
using TodoServicesJWTAPI.Data;
using TodoServicesJWTAPI.Models.DTOs.Pagintions;
using TodoServicesJWTAPI.Models.DTOs.Product;
using TodoServicesJWTAPI.Models.DTOs.Todo;
using TodoServicesJWTAPI.Models.Entities;

namespace TodoServicesJWTAPI.Services.Product
{
    public class ProductService : IProductService
    {
        private readonly TodoDbContext _context;

        public ProductService(TodoDbContext context)
        {
            _context = context;
        }




        public async Task<PagintionListDto<ProductItemDto>> FilterAll(PagintionRequest request, Category? category, string? productDescAsc, int? minPrice, int? maxPrice)
        {
            try
            {
                var query = _context.Products.Include(c => c.Category).AsQueryable();
                if (category != null)
                    query = query.Where(c => c.CategoryId == category.Id);
                if (minPrice.HasValue)
                    query = query.Where(p => p.Price >= minPrice.Value);
                if (maxPrice.HasValue)
                    query = query.Where(p => p.Price <= maxPrice.Value);
                if (!string.IsNullOrEmpty(productDescAsc))
                {
                    switch (productDescAsc.ToLower())
                    {
                        case "asc":
                            query = query.OrderBy(p => p.Price);
                            break;
                        case "desc":
                            query = query.OrderByDescending(p => p.Price);
                            break;
                        default:
                            break;
                    }
                }
                var items = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
                var totalCount = await query.CountAsync();

                return new PagintionListDto<ProductItemDto>(
                    items.Select(p => new ProductItemDto
                    (
                        p.Title, 
                        p.Description, 
                        p.Price, 
                        p.CategoryId)),
                    new PagintionMeta(
                        request.Page, 
                        request.PageSize, 
                        totalCount)
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Get All Products:{ex.Message}");
                throw;
            }

        }







        public async Task<ProductItemDto> CreateProduct(CreateProductRequest request)
        {
            try
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId) ?? throw new NullReferenceException("Category not exist!");

                ProductItem product = new ()
                {
                    Title = request.Title,
                    Description = request.Description,
                    Price = request.Price,
                    CategoryId = request.CategoryId
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return new ProductItemDto(
                    title: product.Title,
                    price: product.Price,
                    description: product.Description,
                    categoryId: product.CategoryId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Create Product:{ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                var productItem = await _context.Products.FirstOrDefaultAsync(e => e.Id == id);
                if (productItem == null)
                {
                    return false;
                }

                _context.Products.Remove(productItem);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete Product:{ex.Message}");
                throw;
            }
        }

        public async Task<ProductItemDto?> GetProduct(int id)
        {
            try
            {
                var productItem = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(e => e.Id == id);

                return productItem is not null
                    ? new ProductItemDto(
                        title: productItem.Title,
                        price: productItem.Price,
                        description: productItem.Description,
                        categoryId: productItem.CategoryId)   
                    : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Get Product:{ex.Message}");
                throw;
            }
        }
    }
    
}
