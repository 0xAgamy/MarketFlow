using API.DTOs;
using Core.Entities;

namespace MarketFlow.API.Mapping;

public static class ProductMappingExtensions
{
    public static Product ToEntity(this CreateProductDTO dto) => new()
    {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            PictureUrl = dto.PictureUrl,
            Type = dto.Type,
            Brand = dto.Brand,
            QuantityInStock = dto.QuantityInStock
    };


    public static void UpdateFrom(this Product product, CreateProductDTO dto)
    {
        product.Name = dto.Name;
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.PictureUrl = dto.PictureUrl;
        product.Type = dto.Type;
        product.Brand = dto.Brand;
        product.QuantityInStock = dto.QuantityInStock;
    }
}
