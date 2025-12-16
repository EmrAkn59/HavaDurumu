using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HavaDurumu.Data.Entities.ViewModels
{
    [Table("vw_ModelPerformanceCheck")]
    public class ModelPerformanceCheckView
    {
        public int StationID { get; set; }
        public DateTime? PredictedDate { get; set; }
        public decimal? PredictedValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? Difference { get; set; }
    }
}

