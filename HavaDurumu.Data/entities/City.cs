using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Data.Entities
{
    [Table("Cities")]
    public class City
    {
        [Key]
        public int CityID { get; set; }

        [Required]
        [StringLength(100)]
        public string CityName { get; set; }

        [StringLength(3)]
        public string CountryCode { get; set; } = "TUR"; // Default değer

        // İlişki: Bir şehirde birden fazla istasyon olabilir
        public virtual ICollection<Station> Stations { get; set; }
    }
}
