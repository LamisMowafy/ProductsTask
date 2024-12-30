using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Common
{
    public abstract class AuditableEntity
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [JsonIgnore]
        public long CreatedBy { get; set; }
        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
        [JsonIgnore]
        public long ModifiedBy { get; set; }
        [JsonIgnore]
        public DateTime ModifiedOn { get; set; }
        [JsonIgnore]
        public bool IsDeleted { get; set; }
    }
}
