using HavaDurumu.Data.Entities.ViewModels;
using System;
using System.Collections.Generic;

namespace HavaDurumu.DataAccess.Interfaces
{
    public interface IReportDal
    {
        // Stored Procedure: Şehir raporu
        List<CityReportResult> GetCityReport(string cityName, DateTime startDate, DateTime endDate);

        // Function: AQI hesaplama
        int CalculateAQI(decimal pm25);

        // Function: Aktif alarm sayısı
        int GetActiveAlertCount(int stationID);
    }
}

