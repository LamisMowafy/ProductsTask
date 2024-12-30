using Domain.Common;

namespace Domain.Models
{
    public class ProductPromotion : AuditableEntity
    {
        public long ProductId { get; set; }
        public Products Product { get; set; }

        public long PromotionId { get; set; }
        public Promotion Promotion { get; set; }
    }
}
