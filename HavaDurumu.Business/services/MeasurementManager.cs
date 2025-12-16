using HavaDurumu.Business.Abstract;
using HavaDurumu.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Business.Services
{
    public class MeasurementManager : IMeasurementService
    {
        IMeasurementDal _measurementDal;

        public MeasurementManager(IMeasurementDal measurementDal)
        {
            _measurementDal = measurementDal;
        }

        // Stored Procedure ile ölçüm ekleme
        public void AddMeasurement(int stationID, decimal pm25, decimal co2, decimal temp, decimal humidity)
        {
            if (stationID <= 0)
            {
                throw new ArgumentException("Geçerli bir istasyon ID'si giriniz.", nameof(stationID));
            }

            if (pm25 < 0 || co2 < 0)
            {
                throw new ArgumentException("PM2.5 ve CO2 değerleri negatif olamaz.");
            }

            if (temp < -50 || temp > 60)
            {
                throw new ArgumentException("Sıcaklık değeri -50 ile 60 arasında olmalıdır.");
            }

            if (humidity < 0 || humidity > 100)
            {
                throw new ArgumentException("Nem değeri 0 ile 100 arasında olmalıdır.");
            }

            _measurementDal.AddMeasurement(stationID, pm25, co2, temp, humidity);
        }

        // CRUD işlemleri
        public void MeasurementAdd(HavaDurumu.Data.Entities.Measurement measurement)
        {
            if (measurement == null)
            {
                throw new ArgumentNullException(nameof(measurement), "Ölçüm bilgisi boş olamaz.");
            }

            if (measurement.StationID <= 0)
            {
                throw new ArgumentException("Geçerli bir istasyon ID'si giriniz.", nameof(measurement));
            }

            _measurementDal.Insert(measurement);
        }

        public void MeasurementDelete(HavaDurumu.Data.Entities.Measurement measurement)
        {
            if (measurement == null)
            {
                throw new ArgumentNullException(nameof(measurement), "Ölçüm bilgisi boş olamaz.");
            }

            _measurementDal.Delete(measurement);
        }

        public HavaDurumu.Data.Entities.Measurement GetById(long id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Geçerli bir ölçüm ID'si giriniz.", nameof(id));
            }

            return _measurementDal.GetById(id);
        }

        public List<HavaDurumu.Data.Entities.Measurement> GetList()
        {
            return _measurementDal.GetList();
        }

        public void MeasurementUpdate(HavaDurumu.Data.Entities.Measurement measurement)
        {
            if (measurement == null)
            {
                throw new ArgumentNullException(nameof(measurement), "Ölçüm bilgisi boş olamaz.");
            }

            _measurementDal.Update(measurement);
        }
    }
}

