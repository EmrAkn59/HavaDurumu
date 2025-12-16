using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Data.Entities
{
    [Table("Alerts")]
    public class Alert
    {
        [Key]
        public long AlertID { get; set; } // BIGINT -> long

        public int StationID { get; set; }
        [ForeignKey("StationID")]
        public virtual Station Station { get; set; }

        [Required]
        [StringLength(255)]
        public string AlertMessage { get; set; }

        public byte? SeverityLevel { get; set; } // TINYINT -> byte

        public bool IsResolved { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
