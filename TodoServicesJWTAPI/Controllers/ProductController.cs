using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TodoServicesJWTAPI.Models.DTOs.Pagintions;
using TodoServicesJWTAPI.Models.DTOs.Product;
using TodoServicesJWTAPI.Models.DTOs.Todo;
using TodoServicesJWTAPI.Models.Entities;
using TodoServicesJWTAPI.Providers;
using TodoServicesJWTAPI.Services.Product;
using TodoServicesJWTAPI.Services.Todo;

namespace TodoServicesJWTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMemoryCache _memoryCache;
        public ProductController(IProductService productService, IMemoryCache memoryCache)
        {
            _productService = productService;
            _memoryCache = memoryCache;
        }

        [HttpGet("get/{id}")]
        public async Task<ActionResult<ProductItemDto>> Get(int id)
        {
            try
            {
                if (_memoryCache.TryGetValue<ProductItemDto>($"product_{id}", out var cachedProduct))
                    return Ok(cachedProduct);
                else
                {
                    var item = await _productService.GetProduct(id);
                    if (item is not null)
                    {
                        _memoryCache.Set(
                            key: $"product_{id}",
                            value: item,
                            options: new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300) });

                        return item;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<ProductItemDto>> Create([FromBody] CreateProductRequest request)
        {
            try
            {
                if (_memoryCache.TryGetValue<ProductItemDto>($"product_{request.Title}", out var cachedProduct))
                    return Ok(cachedProduct);

                var item = await _productService.CreateProduct(request);
                if (item is not null)
                {
                    _memoryCache.Set(
                        key: $"product_{request.Title}",
                        value: item,
                        options: new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300) });

                    return item;
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }


        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ProductItemDto>> Delete(int id)
        {
            try
            {
                var item = await (_productService.DeleteProduct(id));
                return item
                    ? NoContent()
                    : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }

        [HttpGet("filter")]
        public async Task<ActionResult<PagintionListDto<ProductItemDto>>> FilterAll([FromQuery] PagintionRequest request, [FromQuery] Category? category, [FromQuery] string? productDescAsc, [FromQuery] int? minPrice, [FromQuery] int? maxPrice)
        {
            try
            {
                var cacheKey = $"filtered_products_{request.Page}_{request.PageSize}_{category?.Id ?? 0}_{productDescAsc}_{minPrice ?? 0}_{maxPrice ?? 0}";

                if (_memoryCache.TryGetValue<PagintionListDto<ProductItemDto>>(cacheKey, out var cachedResult))
                    return Ok(cachedResult);
                else
                {
                    var result = await _productService.FilterAll(request, category, productDescAsc, minPrice, maxPrice);
                    if (result is not null)
                    {
                        _memoryCache.Set(
                            key: cacheKey,
                            value: result,
                            options: new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300) });

                        return Ok(result);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

    }
}
