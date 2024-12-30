using Application.Services.Product.Command.Create;
using Application.Services.Product.Command.Delete;
using Application.Services.Product.Command.Update;
using AutoMapper;
using Domain.DTOs.Product;
using Domain.Models;


namespace Application.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Products, ProductDto>().ReverseMap();
            CreateMap<Products, CreateProduct>().ReverseMap();
            CreateMap<Products, UpdateProduct>().ReverseMap();
            CreateMap<Products, DeleteProduct>().ReverseMap();
        }
    }
}
