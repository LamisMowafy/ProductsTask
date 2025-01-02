using Application.Services.Product.Command.Create;
using Application.Services.Product.Command.Delete;
using Application.Services.Product.Command.Update;
using Application.Services.Product.Queries.GetDetail;
using Application.Services.Promotion.Command.Create;
using Application.Services.Promotion.Command.Delete;
using Application.Services.Promotion.Command.Update;
using AutoMapper;
using Domain.DTOs.Product;
using Domain.DTOs.Promotion;
using Domain.Models;


namespace Application.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // products 
            CreateMap<Products, ProductDto>().ReverseMap();
            CreateMap<Products, CreateProduct>().ReverseMap();
            CreateMap<Products, UpdateProduct>().ReverseMap();
            CreateMap<Products, DeleteProduct>().ReverseMap();
            CreateMap<Products, ProductDetailDto>().ReverseMap();
            // promotions
            CreateMap<Promotions, CreatePromotion>().ReverseMap();
            CreateMap<Promotions, UpdatePromotion>().ReverseMap();
            CreateMap<Promotions, DeletePromotion>().ReverseMap();
            CreateMap<Promotions, PromotionDto>().ReverseMap();

        }
    }
}
