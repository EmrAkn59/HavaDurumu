using HavaDurumu.DataAccess.Interfaces;
using HavaDurumu.DataAccess.Repositories;
using HavaDurumu.Data.Entities;
using HavaDurumu.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.DataAccess.EntityFramework
{
    public class EfMeasurementDal : GenericRepository<Measurement>, IMeasurementDal
    {
        private AppDbContext _context;

        public EfMeasurementDal(AppDbContext context) : base(context)
        {
            _context = context;
        }

        // Stored Procedure: Ölçüm ekleme
        public void AddMeasurement(int stationID, decimal pm25, decimal co2, decimal temp, decimal humidity)
        {
            _context.Database.ExecuteSqlCommand(
                "EXEC sp_AddMeasurement @StationID, @PM25, @CO2, @Temp, @Humidity",
                new SqlParameter("@StationID", stationID),
                new SqlParameter("@PM25", pm25),
                new SqlParameter("@CO2", co2),
                new SqlParameter("@Temp", temp),
                new SqlParameter("@Humidity", humidity)
            );
        }

        // Measurement için özel GetById (long tipinde ID)
        public Measurement GetById(long id)
        {
            return _context.Set<Measurement>().Find(id);
        }
    }
}

