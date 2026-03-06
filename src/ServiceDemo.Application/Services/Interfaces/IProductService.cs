using System.Threading.Tasks;
using ServiceDemo.Application.Common;
using ServiceDemo.Application.DTOs.Product;

namespace ServiceDemo.Application.Services.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto createProductDto);
        Task<ApiResponse<ProductDto>> GetProductByIdAsync(long id);
        Task<ApiResponse<PagedResult<ProductDto>>> GetProductsAsync(int page = 0, int pageSize = 10);
        Task<ApiResponse<ProductDto>> UpdateProductAsync(long id, UpdateProductDto updateProductDto);
        Task<ApiResponse<bool>> DeleteProductAsync(long id);
    }
}