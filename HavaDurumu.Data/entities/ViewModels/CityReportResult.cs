namespace HavaDurumu.Data.Entities.ViewModels
{
    // Stored Procedure sonuç modeli
    public class CityReportResult
    {
        public string CityName { get; set; }
        public decimal? AveragePollution { get; set; }
        public decimal? MaxPollution { get; set; }
    }
}

