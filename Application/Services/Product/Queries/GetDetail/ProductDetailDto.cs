using Domain.DTOs.Promotion;

namespace Application.Services.Product.Queries.GetDetail
{
    public class ProductDetailDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public List<PromotionDto>  ProductPromotions { get; set; }
    }
}
