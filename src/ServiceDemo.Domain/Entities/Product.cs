using System.ComponentModel.DataAnnotations;
namespace ServiceDemo.Domain.Entities
{
    public class Product
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Stock { get; set; } 
    }
}