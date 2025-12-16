using System;
using System.Runtime.Serialization;

namespace HavaDurumu.Integration.Models
{
    [DataContract]
    public class StationInfo
    {
        [DataMember]
        public int StationID { get; set; }

        [DataMember]
        public string StationName { get; set; }

        [DataMember]
        public string CityName { get; set; }

        [DataMember]
        public decimal? Latitude { get; set; }

        [DataMember]
        public decimal? Longitude { get; set; }

        [DataMember]
        public bool IsActive { get; set; }
    }
}

