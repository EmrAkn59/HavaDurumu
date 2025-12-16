using HavaDurumu.Data.Context;
using HavaDurumu.Data.Entities;
using HavaDurumu.DataAccess.Interfaces;
using HavaDurumu.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.DataAccess.EntityFramework
{
    public class EfUserDal : GenericRepository<User>, IUserDal
    {
        public EfUserDal(AppDbContext context) : base(context)
        {
        }
    }
}
