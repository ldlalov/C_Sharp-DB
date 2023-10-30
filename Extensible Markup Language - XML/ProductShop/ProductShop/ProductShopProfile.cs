using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<ImportedUsers, User>();
            CreateMap<ImportedProduct, Product>();
            CreateMap<Product, ProductsInRange > ();
        }
    }
}
