using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HavaDurumu.Data.Entities.ViewModels
{
    [Table("vw_CriticalUnresolvedAlerts")]
    public class CriticalUnresolvedAlertsView
    {
        public long AlertID { get; set; }
        public string StationName { get; set; }
        public string AlertMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

