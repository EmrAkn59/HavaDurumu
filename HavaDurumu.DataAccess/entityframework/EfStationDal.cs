using HavaDurumu.DataAccess.Interfaces;
using HavaDurumu.DataAccess.Repositories;
using HavaDurumu.Data.Entities;
using HavaDurumu.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.DataAccess.EntityFramework
{
    public class EfStationDal : GenericRepository<Station>, IStationDal
    {
        public EfStationDal(AppDbContext context) : base(context)
        {
        }
    }
}
