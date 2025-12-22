using HavaDurumu.Core.Abstract;
using HavaDurumu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.DataAccess.Interfaces
{
    public interface IMeasurementDal : IGenericDal<Measurement>
    {
        // Stored Procedure: Ölçüm ekleme
        void AddMeasurement(int stationID, decimal pm25, decimal co2, decimal temp, decimal humidity);

        // Measurement için özel GetById (long tipinde ID)
        Measurement GetById(long id);
    }
}
