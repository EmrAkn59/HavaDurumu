using HavaDurumu.Data.Entities.ViewModels;
using System;
using System.Collections.Generic;

namespace HavaDurumu.Business.Abstract
{
    public interface IReportService
    {
        // Stored Procedure: Şehir raporu
        List<CityReportResult> GetCityReport(string cityName, DateTime startDate, DateTime endDate);

        // Function: AQI hesaplama
        int CalculateAQI(decimal pm25);

        // Function: Aktif alarm sayısı
        int GetActiveAlertCount(int stationID);

        // View'lar
        List<StationDetailsView> GetStationDetails();
        List<UserProfilesView> GetUserProfiles();
        List<DailyStationAveragesView> GetDailyStationAverages();
        List<CriticalUnresolvedAlertsView> GetCriticalUnresolvedAlerts();
        List<ModelPerformanceCheckView> GetModelPerformanceCheck();
    }
}

