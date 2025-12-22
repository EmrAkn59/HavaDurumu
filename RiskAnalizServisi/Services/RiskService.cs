using Grpc.Core;
using RiskAnalizServisi;

namespace RiskAnalizServisi.Services
{
    public class RiskService : PollutionRisk.PollutionRiskBase
    {
        private readonly ILogger<RiskService> _logger;

        public RiskService(ILogger<RiskService> logger)
        {
            _logger = logger;
        }

        public override Task<RiskReport> AnalyzeRisk(MeasurementData request, ServerCallContext context)
        {
            _logger.LogInformation($"Analiz Talebi -> ID: {request.MeasureId}, Temp: {request.Temperature}, PM2.5: {request.Pm25Value}, Nem: {request.Humidity}");

            bool isRisky = false;
            string riskLevel = "NORMAL";
            List<string> uyariMesajlari = new List<string>();

            // SICAKLIK ANALIZI
            if (request.Temperature > 40)
            {
                isRisky = true;
                riskLevel = "YUKSEK"; 
                uyariMesajlari.Add("ASIRI SICAK: Gunes carpmasi riski! Disari cikmayin.");
            }
            else if (request.Temperature < -10)
            {
                isRisky = true;
                riskLevel = "YUKSEK"; 
                uyariMesajlari.Add("DONDURUCU SOGUK: Hipotermi riski! Kalin giyinin.");
            }

            // HAVA KIRLILIGI ANALIZI (PM2.5)
            if (request.Pm25Value > 50)
            {
                isRisky = true;
                uyariMesajlari.Add("HAVA KIRLILIGI: Solunum maskesi takmaniz onerilir.");
                riskLevel = "TEHLIKELI";
            }

            // NEM VE ISI BIRLESIMI
            if (request.Temperature > 30 && request.Humidity > 70)
            {
                isRisky = true;
                uyariMesajlari.Add("YUKSEK NEM: Hissedilen sicaklik cok yuksek, sivi tuketin.");
                riskLevel = "YUKSEK";
            }

            string finalMessage = uyariMesajlari.Count > 0
                ? string.Join(" | ", uyariMesajlari)
                : "Hava degerleri mevsim normallerinde. Risk yok.";

            if (uyariMesajlari.Count >= 2)
            {
                riskLevel = "KRITIK";
            }

            return Task.FromResult(new RiskReport
            {
                IsRisky = isRisky,
                RiskLevel = riskLevel,
                Message = finalMessage
            });
        }
    }
}
