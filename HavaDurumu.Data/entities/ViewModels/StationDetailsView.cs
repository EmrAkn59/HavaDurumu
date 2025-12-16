using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HavaDurumu.Data.Entities.ViewModels
{
    [Table("vw_StationDetails")]
    public class StationDetailsView
    {
        public int StationID { get; set; }
        public string StationName { get; set; }
        public string CityName { get; set; }
        public string CountryCode { get; set; }
        public bool IsActive { get; set; }
    }
}

