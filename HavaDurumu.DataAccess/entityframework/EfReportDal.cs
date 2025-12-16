using HavaDurumu.DataAccess.Interfaces;
using HavaDurumu.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using HavaDurumu.Data.Entities.ViewModels;

namespace HavaDurumu.DataAccess.EntityFramework
{
    public class EfReportDal : IReportDal
    {
        private AppDbContext _context;

        public EfReportDal(AppDbContext context)
        {
            _context = context;
        }

        // Stored Procedure: Şehir raporu
        public List<CityReportResult> GetCityReport(string cityName, DateTime startDate, DateTime endDate)
        {
            var cityNameParam = new SqlParameter("@CityName", cityName);
            var startDateParam = new SqlParameter("@StartDate", startDate);
            var endDateParam = new SqlParameter("@EndDate", endDate);

            var data = _context.Database.SqlQuery<CityReportResult>(
                "EXEC sp_GetCityReport @CityName, @StartDate, @EndDate",
                cityNameParam,
                startDateParam,
                endDateParam
            ).ToList();

            return data;
        }

        // Function: AQI hesaplama
        public int CalculateAQI(decimal pm25)
        {
            var pm25Param = new SqlParameter("@pm25", pm25);
            var result = _context.Database.SqlQuery<int>(
                "SELECT dbo.fn_CalculateAQI(@pm25)",
                pm25Param
            ).FirstOrDefault();

            return result;
        }

        // Function: Aktif alarm sayısı
        public int GetActiveAlertCount(int stationID)
        {
            var stationIDParam = new SqlParameter("@StationID", stationID);
            var result = _context.Database.SqlQuery<int>(
                "SELECT dbo.fn_GetActiveAlertCount(@StationID)",
                stationIDParam
            ).FirstOrDefault();

            return result;
        }
    }
}

