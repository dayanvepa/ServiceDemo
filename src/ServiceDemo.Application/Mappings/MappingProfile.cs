using AutoMapper;
using ServiceDemo.Application.DTOs.Product;
using ServiceDemo.Domain.Entities;


namespace ServiceDemo.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for entity-DTO mappings
    /// </summary>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}