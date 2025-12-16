using System;
using System.Runtime.Serialization;

namespace HavaDurumu.Integration.Models
{
    [DataContract]
    public class MeasurementInfo
    {
        [DataMember]
        public long MeasureID { get; set; }

        [DataMember]
        public int StationID { get; set; }

        [DataMember]
        public string StationName { get; set; }

        [DataMember]
        public DateTime MeasureDate { get; set; }

        [DataMember]
        public decimal? PM25_Value { get; set; }

        [DataMember]
        public decimal? CO2_Value { get; set; }

        [DataMember]
        public decimal? Temperature { get; set; }

        [DataMember]
        public decimal? Humidity { get; set; }

        [DataMember]
        public int? AQI { get; set; }
    }
}

