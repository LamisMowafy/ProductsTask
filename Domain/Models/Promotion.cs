using Domain.Common;

namespace Domain.Models
{
    public class Promotions : AuditableEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal DiscountPercentage { get; set; }
        // Navigation properties
        public ICollection<ProductPromotion> ProductPromotions { get; set; }
    }
}
