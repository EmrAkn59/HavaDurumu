using HavaDurumu.Core.Abstract;
using HavaDurumu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.DataAccess.Interfaces
{
    public interface IStationDal : IGenericDal<Station>
    {
        // İleride buraya özel metodlar yazacağız.
        // Örnek: List<Station> GetActiveStations(); 
    }
}
