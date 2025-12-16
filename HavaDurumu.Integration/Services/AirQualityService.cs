using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using HavaDurumu.Integration.Contracts;
using HavaDurumu.Integration.Models;
using HavaDurumu.Business.Abstract;
using HavaDurumu.Business.Services;
using HavaDurumu.DataAccess.Interfaces;
using HavaDurumu.DataAccess.EntityFramework;
using HavaDurumu.Data.Context;
using HavaDurumu.Data.Entities;

namespace HavaDurumu.Integration.Services
{
    public class AirQualityService : IAirQualityService
    {
        private IStationService _stationService;
        private IMeasurementService _measurementService;
        private IReportService _reportService;
        private AppDbContext _context;

        public AirQualityService()
        {
            // Dependency Injection yerine manuel oluşturma
            _context = new AppDbContext();
            
            var stationDal = new EfStationDal(_context);
            _stationService = new StationManager(stationDal);

            var measurementDal = new EfMeasurementDal(_context);
            _measurementService = new MeasurementManager(measurementDal);

            var reportDal = new EfReportDal(_context);
            _reportService = new ReportManager(reportDal, _context);
        }

        public List<StationInfo> GetAllActiveStations()
        {
            try
            {
                var stations = _context.Stations
                    .Include("City")
                    .Where(s => s.IsActive)
                    .ToList();

                var result = new List<StationInfo>();
                foreach (var station in stations)
                {
                    result.Add(new StationInfo
                    {
                        StationID = station.StationID,
                        StationName = station.StationName,
                        CityName = station.City?.CityName ?? "Bilinmeyen",
                        Latitude = station.Latitude,
                        Longitude = station.Longitude,
                        IsActive = station.IsActive
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new FaultException($"İstasyonlar getirilirken hata oluştu: {ex.Message}");
            }
        }

        public StationInfo GetStationById(int stationId)
        {
            try
            {
                var station = _context.Stations
                    .Include("City")
                    .FirstOrDefault(s => s.StationID == stationId);
                
                if (station == null)
                {
                    throw new FaultException($"ID={stationId} olan istasyon bulunamadı.");
                }

                return new StationInfo
                {
                    StationID = station.StationID,
                    StationName = station.StationName,
                    CityName = station.City?.CityName ?? "Bilinmeyen",
                    Latitude = station.Latitude,
                    Longitude = station.Longitude,
                    IsActive = station.IsActive
                };
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FaultException($"İstasyon bilgisi getirilirken hata oluştu: {ex.Message}");
            }
        }

        public List<MeasurementInfo> GetStationMeasurements(int stationId, int count)
        {
            try
            {
                var measurements = _measurementService.GetList()
                    .Where(m => m.StationID == stationId)
                    .OrderByDescending(m => m.MeasureDate)
                    .Take(count > 0 ? count : 10)
                    .ToList();

                var result = new List<MeasurementInfo>();
                var station = _stationService.TGetById(stationId);
                
                foreach (var measurement in measurements)
                {
                    int? aqi = null;
                    if (measurement.PM25_Value.HasValue)
                    {
                        aqi = _reportService.CalculateAQI(measurement.PM25_Value.Value);
                    }

                    result.Add(new MeasurementInfo
                    {
                        MeasureID = measurement.MeasureID,
                        StationID = measurement.StationID,
                        StationName = station?.StationName ?? "Bilinmeyen",
                        MeasureDate = measurement.MeasureDate,
                        PM25_Value = measurement.PM25_Value,
                        CO2_Value = measurement.CO2_Value,
                        Temperature = measurement.Temperature,
                        Humidity = measurement.Humidity,
                        AQI = aqi
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new FaultException($"Ölçümler getirilirken hata oluştu: {ex.Message}");
            }
        }

        public CityReportInfo GetCityReport(string cityName, DateTime startDate, DateTime endDate)
        {
            try
            {
                var reports = _reportService.GetCityReport(cityName, startDate, endDate);
                var report = reports.FirstOrDefault();

                if (report == null)
                {
                    return new CityReportInfo
                    {
                        CityName = cityName,
                        AveragePollution = null,
                        MaxPollution = null
                    };
                }

                return new CityReportInfo
                {
                    CityName = report.CityName,
                    AveragePollution = report.AveragePollution,
                    MaxPollution = report.MaxPollution
                };
            }
            catch (Exception ex)
            {
                throw new FaultException($"Şehir raporu getirilirken hata oluştu: {ex.Message}");
            }
        }

        public int CalculateAQI(decimal pm25)
        {
            try
            {
                return _reportService.CalculateAQI(pm25);
            }
            catch (Exception ex)
            {
                throw new FaultException($"AQI hesaplanırken hata oluştu: {ex.Message}");
            }
        }

        public int GetActiveAlertCount(int stationId)
        {
            try
            {
                return _reportService.GetActiveAlertCount(stationId);
            }
            catch (Exception ex)
            {
                throw new FaultException($"Aktif alarm sayısı getirilirken hata oluştu: {ex.Message}");
            }
        }

        public bool AddMeasurement(int stationId, decimal pm25, decimal co2, decimal temp, decimal humidity)
        {
            try
            {
                _measurementService.AddMeasurement(stationId, pm25, co2, temp, humidity);
                return true;
            }
            catch (Exception ex)
            {
                throw new FaultException($"Ölçüm eklenirken hata oluştu: {ex.Message}");
            }
        }
    }
}

