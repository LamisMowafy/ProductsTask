using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class ProductPromotion : AuditableEntity
    {
        public long ProductId { get; set; }
        public Products Product { get; set; }
        public long PromotionId { get; set; }
        public Promotions Promotion { get; set; }
    }
}
