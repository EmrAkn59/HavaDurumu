using System.Collections.Generic;
using System.ServiceModel;
using HavaDurumu.Integration.Models;

namespace HavaDurumu.Integration.Contracts
{
    [ServiceContract(Namespace = "http://havadurumu.com/soap")]
    public interface IAirQualityService
    {
        /// <summary>
        /// Tüm aktif istasyonları getirir
        /// </summary>
        [OperationContract]
        List<StationInfo> GetAllActiveStations();

        /// <summary>
        /// Belirli bir istasyonun bilgilerini getirir
        /// </summary>
        [OperationContract]
        StationInfo GetStationById(int stationId);

        /// <summary>
        /// Belirli bir istasyonun son ölçümlerini getirir
        /// </summary>
        [OperationContract]
        List<MeasurementInfo> GetStationMeasurements(int stationId, int count);

        /// <summary>
        /// Belirli bir şehir için rapor getirir
        /// </summary>
        [OperationContract]
        CityReportInfo GetCityReport(string cityName, System.DateTime startDate, System.DateTime endDate);

        /// <summary>
        /// PM2.5 değerine göre AQI (Air Quality Index) hesaplar
        /// </summary>
        [OperationContract]
        int CalculateAQI(decimal pm25);

        /// <summary>
        /// Belirli bir istasyon için aktif alarm sayısını getirir
        /// </summary>
        [OperationContract]
        int GetActiveAlertCount(int stationId);

        /// <summary>
        /// Yeni ölçüm ekler (Stored Procedure kullanarak)
        /// </summary>
        [OperationContract]
        bool AddMeasurement(int stationId, decimal pm25, decimal co2, decimal temp, decimal humidity);
    }
}

