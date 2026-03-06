using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using ServiceDemo.Application.DTOs.Product;
using ServiceDemo.Application.Services;
using ServiceDemo.Domain.Entities;
using ServiceDemo.Domain.Interfaces;
using Xunit;

namespace ServiceDemo.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        // Conectar el repositorio al UnitOfWork
        _unitOfWorkMock.Setup(u => u.Products).Returns(_productRepositoryMock.Object);

        _productService = new ProductService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    // ─────────────────────────────────────────────
    // CREATE
    // ─────────────────────────────────────────────

    [Fact]
    public async Task CreateProductAsync_ValidDto_ReturnsSuccess()
    {
        // Arrange
        var createDto = new CreateProductDto { Name = "Laptop", Price = 1500, Stock = 10 };
        var product   = new Product { Id = 1, Name = "Laptop", Price = 1500, Stock = 10 };
        var productDto = new ProductDto { Id = 1, Name = "Laptop", Price = 1500, Stock = 10 };

        _mapperMock.Setup(m => m.Map<Product>(createDto)).Returns(product);
        _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);
        _productRepositoryMock.Setup(r => r.CreateAsync(product)).ReturnsAsync(product);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _productService.CreateProductAsync(createDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Product created successfully", result.Message);
        Assert.Equal(1, result.Data!.Id);
        Assert.Equal("Laptop", result.Data.Name);
        Assert.Equal(1500, result.Data.Price);
        Assert.Equal(10, result.Data.Stock);
        _productRepositoryMock.Verify(r => r.CreateAsync(product), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_WhenRepositoryThrows_ReturnsError()
    {
        // Arrange
        var createDto = new CreateProductDto { Name = "Laptop", Price = 1500, Stock = 10 };
        var product   = new Product { Id = 1, Name = "Laptop", Price = 1500, Stock = 10 };

        _mapperMock.Setup(m => m.Map<Product>(createDto)).Returns(product);
        _productRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _productService.CreateProductAsync(createDto);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("DB error", result.Message);
    }

    // ─────────────────────────────────────────────
    // GET BY ID
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetProductByIdAsync_ExistingId_ReturnsProduct()
    {
        // Arrange
        var product    = new Product { Id = 1, Name = "Laptop", Price = 1500, Stock = 10 };
        var productDto = new ProductDto { Id = 1, Name = "Laptop", Price = 1500, Stock = 10 };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _mapperMock.Setup(m => m.Map<ProductDto>(product)).Returns(productDto);

        // Act
        var result = await _productService.GetProductByIdAsync(1);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.Data!.Id);
        Assert.Equal("Laptop", result.Data.Name);
        Assert.Equal(1500, result.Data.Price);
        Assert.Equal(10, result.Data.Stock);
    }

    [Fact]
    public async Task GetProductByIdAsync_NonExistingId_ReturnsError()
    {
        // Arrange
        _productRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetProductByIdAsync(99);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Product not found", result.Message);
    }

    [Fact]
    public async Task GetProductByIdAsync_WhenRepositoryThrows_ReturnsError()
    {
        // Arrange
        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<long>()))
            .ThrowsAsync(new Exception("Connection error"));

        // Act
        var result = await _productService.GetProductByIdAsync(1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Connection error", result.Message);
    }

    // ─────────────────────────────────────────────
    // GET PAGED
    // ─────────────────────────────────────────────

    [Fact]
    public async Task GetProductsAsync_ReturnsPagedResult()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Price = 1500, Stock = 10 },
            new() { Id = 2, Name = "Mouse",  Price = 25,   Stock = 50 }
        };
        var productDtos = new List<ProductDto>
        {
            new() { Id = 1, Name = "Laptop", Price = 1500, Stock = 10 },
            new() { Id = 2, Name = "Mouse",  Price = 25,   Stock = 50 }
        };

        _productRepositoryMock.Setup(r => r.GetPagedAsync(0, 10)).ReturnsAsync(products);
        _productRepositoryMock.Setup(r => r.CountAsync()).ReturnsAsync(2);
        _mapperMock.Setup(m => m.Map<List<ProductDto>>(products)).Returns(productDtos);

        // Act
        var result = await _productService.GetProductsAsync(0, 10);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.TotalCount);
        Assert.Equal(2, result.Data.Items.Count);
    }

    [Fact]
    public async Task GetProductsAsync_WhenRepositoryThrows_ReturnsError()
    {
        // Arrange
        _productRepositoryMock
            .Setup(r => r.GetPagedAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception("Timeout"));

        // Act
        var result = await _productService.GetProductsAsync();

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Timeout", result.Message);
    }

    // ─────────────────────────────────────────────
    // UPDATE
    // ─────────────────────────────────────────────

    [Fact]
    public async Task UpdateProductAsync_ExistingProduct_ReturnsSuccess()
    {
        // Arrange
        var existingProduct = new Product { Id = 1, Name = "Old Name", Price = 100, Stock = 5 };
        var updateProduct =new Product { Id = 1, Name = "New Name", Price = 200, Stock = 20 };
        var updateDto       = new UpdateProductDto { Name = "New Name", Price = 200, Stock = 20 };
        var updatedDto      = new ProductDto { Id = 1, Name = "New Name", Price = 200, Stock = 20 };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(r => r.UpdateAsync(updateProduct)).ReturnsAsync(updateProduct);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<ProductDto>(existingProduct)).Returns(updatedDto);

        // Act
        var result = await _productService.UpdateProductAsync(1, updateDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("New Name", result.Data!.Name);
        Assert.Equal(200, result.Data.Price);
        Assert.Equal(20, result.Data.Stock);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_NonExistingProduct_ReturnsError()
    {
        // Arrange
        _productRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.UpdateProductAsync(99, new UpdateProductDto());

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Product not found", result.Message);
    }

    // ─────────────────────────────────────────────
    // DELETE
    // ─────────────────────────────────────────────

    [Fact]
    public async Task DeleteProductAsync_ExistingProduct_ReturnsSuccess()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Laptop", Price = 1500, Stock = 10 };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _productRepositoryMock.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _productService.DeleteProductAsync(1);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.Data);
        _productRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_NonExistingProduct_ReturnsError()
    {
        // Arrange
        _productRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.DeleteProductAsync(99);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Product not found", result.Message);
    }

    [Fact]
    public async Task DeleteProductAsync_WhenRepositoryThrows_ReturnsError()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Laptop", Price = 1500, Stock = 10 };

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _productRepositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<long>()))
            .ThrowsAsync(new Exception("Delete failed"));

        // Act
        var result = await _productService.DeleteProductAsync(1);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Delete failed", result.Message);
    }
}