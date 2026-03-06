namespace ServiceDemo.Application.DTOs.Product
{
    public class ProductDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; } 
    }
}