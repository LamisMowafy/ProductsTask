using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public  class Products : AuditableEntity
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? NewedUntil { get; set; }  // Optional end date for the "New"
        public ICollection<ProductPromotion> ProductPromotions { get; set; } = [];
    }
}
