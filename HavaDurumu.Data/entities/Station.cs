using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Data.Entities
{
    [Table("Stations")]
    public class Station
    {
        [Key]
        public int StationID { get; set; }

        // Foreign Key
        public int CityID { get; set; }
        [ForeignKey("CityID")]
        public virtual City City { get; set; }

        [Required]
        [StringLength(100)]
        public string StationName { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public bool IsActive { get; set; } = true;

        // İstasyonun ilişkili olduğu diğer tablolar
        public virtual ICollection<Measurement> Measurements { get; set; }
        public virtual ICollection<Prediction> Predictions { get; set; }
        public virtual ICollection<Alert> Alerts { get; set; }
    }
}
