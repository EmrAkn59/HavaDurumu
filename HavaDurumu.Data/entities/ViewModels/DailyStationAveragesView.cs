using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HavaDurumu.Data.Entities.ViewModels
{
    [Table("vw_DailyStationAverages")]
    public class DailyStationAveragesView
    {
        public string StationName { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal? AvgPM25 { get; set; }
        public decimal? AvgTemp { get; set; }
        public int? AQI_Status { get; set; }
    }
}

