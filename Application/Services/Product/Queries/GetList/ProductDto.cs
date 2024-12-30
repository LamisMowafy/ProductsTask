using Domain.DTOs.Promotion;

namespace Domain.DTOs.Product
{
    public class ProductDto
    {
        
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public List<PromotionDto> Promotions { get; set; }
    }
}
