using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Data.Entities
{
    [Table("Predictions")]
    public class Prediction
    {
        [Key]
        public long PredictionID { get; set; } // BIGINT -> long

        public int StationID { get; set; }
        [ForeignKey("StationID")]
        public virtual Station Station { get; set; }

        public DateTime? PredictedDate { get; set; }

        // Fluent API ile precision ayarlanacak
        public decimal? PredictedValue { get; set; }

        [StringLength(50)]
        public string ModelVersion { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
