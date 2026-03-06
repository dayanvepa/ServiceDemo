using AutoMapper;
using ServiceDemo.Application.DTOs.Product;
using ServiceDemo.Application.Common;
using ServiceDemo.Domain.Entities;
using ServiceDemo.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using ServiceDemo.Application.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;   

namespace ServiceDemo.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto createProductDto)
        {
            try
            {
                _logger.LogInformation("Creating new product with name: {Name}", createProductDto.Name);
                var product = _mapper.Map<Product>(createProductDto);

                await _unitOfWork.Products.CreateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                var productResponse = _mapper.Map<ProductDto>(product);
                _logger.LogInformation("Product created successfully with ID: {ProductId}", product.Id);

                return ApiResponse<ProductDto>.SuccessResponse(productResponse, "Product created successfully");
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                 return ApiResponse<ProductDto>.ErrorResponse("Error creating product: " + ex.Message);
            }
            
        }

        public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(long id)
        {
            try
            {
                _logger.LogInformation("Getting product with ID: {ProductId}", id);

                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return ApiResponse<ProductDto>.ErrorResponse("Product not found");
                }

                var productResponse = _mapper.Map<ProductDto>(product);
                return ApiResponse<ProductDto>.SuccessResponse(productResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product with ID: {ProductId}", id);
                return ApiResponse<ProductDto>.ErrorResponse("Error retrieving product: " + ex.Message);
            }
        }

        public async Task<ApiResponse<PagedResult<ProductDto>>> GetProductsAsync(int page = 0, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Getting products - Page: {Page}, PageSize: {PageSize}", page, pageSize);

                var products = await _unitOfWork.Products.GetPagedAsync(page, pageSize);
                var totalCount = await _unitOfWork.Products.CountAsync();

                var productResponses = _mapper.Map<List<ProductDto>>(products);

                var pagedResult = new PagedResult<ProductDto>
                {
                    Items = productResponses,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize
                };

                return ApiResponse<PagedResult<ProductDto>>.SuccessResponse(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting products - Page: {Page}, PageSize: {PageSize}", page, pageSize);
                return ApiResponse<PagedResult<ProductDto>>.ErrorResponse("Error retrieving products: " + ex.Message);
            }
        }

        public async Task<ApiResponse<ProductDto>> UpdateProductAsync(long id, UpdateProductDto updateProductDto)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", id);

                var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for update", id);
                    return ApiResponse<ProductDto>.ErrorResponse("Product not found");
                }

                // Update properties
                existingProduct.Name = updateProductDto.Name;
                existingProduct.Price = updateProductDto.Price;
                existingProduct.Stock = updateProductDto.Stock;

                await _unitOfWork.Products.UpdateAsync(existingProduct);
                await _unitOfWork.SaveChangesAsync();

                var productResponse = _mapper.Map<ProductDto>(existingProduct);

                _logger.LogInformation("Product updated successfully with ID: {ProductId}", id);

                return ApiResponse<ProductDto>.SuccessResponse(productResponse, "Product updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {ProductId}", id);
                return ApiResponse<ProductDto>.ErrorResponse("Error updating product: " + ex.Message);
            }
        }

          public async Task<ApiResponse<bool>> DeleteProductAsync(long id)
        {
            try
            {
                _logger.LogInformation("Deleting product with ID: {ProductId}", id);

                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found for deletion", id);
                    return ApiResponse<bool>.ErrorResponse("Product not found");
                }


                await _unitOfWork.Products.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Product deleted successfully with ID: {ProductId}", id);

                return ApiResponse<bool>.SuccessResponse(true, "Product deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {ProductId}", id);
                return ApiResponse<bool>.ErrorResponse("Error deleting product: " + ex.Message);
            }
        }

    }
}