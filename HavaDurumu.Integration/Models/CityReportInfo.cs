using System.Runtime.Serialization;

namespace HavaDurumu.Integration.Models
{
    [DataContract]
    public class CityReportInfo
    {
        [DataMember]
        public string CityName { get; set; }

        [DataMember]
        public decimal? AveragePollution { get; set; }

        [DataMember]
        public decimal? MaxPollution { get; set; }
    }
}

