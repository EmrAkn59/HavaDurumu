using HavaDurumu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Business.Abstract
{
    public interface IStationService
    {
        void TAdd(Station t);
        void TDelete(Station t);
        void TUpdate(Station t);
        List<Station> TGetList();
        Station TGetById(int id);

        // İleride özel metodlar lazım olursa buraya ekleriz
        // Örn: List<Station> GetActiveStations();
    }
}
