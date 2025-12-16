using HavaDurumu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Business.Abstract
{
    public interface IMeasurementService
    {
        // Stored Procedure ile ölçüm ekleme
        void AddMeasurement(int stationID, decimal pm25, decimal co2, decimal temp, decimal humidity);

        // CRUD işlemleri
        void MeasurementAdd(Measurement measurement);
        void MeasurementDelete(Measurement measurement);
        void MeasurementUpdate(Measurement measurement);
        List<Measurement> GetList();
        Measurement GetById(long id);
    }
}

