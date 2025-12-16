using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Data.Entities
{
    [Table("Measurements")]
    public class Measurement
    {
        [Key]
        public long MeasureID { get; set; } // SQL'de BIGINT -> C#'da long

        public int StationID { get; set; }
        [ForeignKey("StationID")]
        public virtual Station Station { get; set; }

        public DateTime MeasureDate { get; set; }

        // SQL'deki decimal(10,2) karşılıkları - Fluent API ile precision ayarlanacak
        public decimal? PM25_Value { get; set; }

        public decimal? CO2_Value { get; set; }

        public decimal? Temperature { get; set; }

        public decimal? Humidity { get; set; }
    }
}
