using HavaDurumu.Business.Abstract;
using HavaDurumu.DataAccess.Interfaces;
using HavaDurumu.Data.Context;
using HavaDurumu.Data.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HavaDurumu.Business.Services
{
    public class ReportManager : IReportService
    {
        IReportDal _reportDal;
        AppDbContext _context;

        public ReportManager(IReportDal reportDal, AppDbContext context)
        {
            _reportDal = reportDal;
            _context = context;
        }

        // Stored Procedure: Şehir raporu
        public List<CityReportResult> GetCityReport(string cityName, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(cityName))
            {
                throw new ArgumentException("Şehir adı boş olamaz.", nameof(cityName));
            }

            if (startDate > endDate)
            {
                throw new ArgumentException("Başlangıç tarihi bitiş tarihinden büyük olamaz.");
            }

            return _reportDal.GetCityReport(cityName, startDate, endDate);
        }

        // Function: AQI hesaplama
        public int CalculateAQI(decimal pm25)
        {
            if (pm25 < 0)
            {
                throw new ArgumentException("PM2.5 değeri negatif olamaz.", nameof(pm25));
            }

            return _reportDal.CalculateAQI(pm25);
        }

        // Function: Aktif alarm sayısı
        public int GetActiveAlertCount(int stationID)
        {
            if (stationID <= 0)
            {
                throw new ArgumentException("Geçerli bir istasyon ID'si giriniz.", nameof(stationID));
            }

            return _reportDal.GetActiveAlertCount(stationID);
        }

        // View: İstasyon detayları
        public List<StationDetailsView> GetStationDetails()
        {
            return _context.StationDetailsViews.ToList();
        }

        // View: Kullanıcı profilleri
        public List<UserProfilesView> GetUserProfiles()
        {
            return _context.UserProfilesViews.ToList();
        }

        // View: Günlük istasyon ortalamaları
        public List<DailyStationAveragesView> GetDailyStationAverages()
        {
            return _context.DailyStationAveragesViews.ToList();
        }

        // View: Kritik çözülmemiş alarmlar
        public List<CriticalUnresolvedAlertsView> GetCriticalUnresolvedAlerts()
        {
            return _context.CriticalUnresolvedAlertsViews.ToList();
        }

        // View: Model performans kontrolü
        public List<ModelPerformanceCheckView> GetModelPerformanceCheck()
        {
            return _context.ModelPerformanceCheckViews.ToList();
        }
    }
}

