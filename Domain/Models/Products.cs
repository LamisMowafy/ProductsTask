using Domain.Common;

namespace Domain.Models
{
    public  class Products : AuditableEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public ICollection<ProductPromotion> ProductPromotions { get; set; }
    }
}
