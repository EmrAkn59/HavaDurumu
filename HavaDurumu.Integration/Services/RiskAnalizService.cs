using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HavaDurumu.Integration
{
    public class RiskAnalizVerisi
    {
        public bool IsRisky { get; set; }
        public string Message { get; set; }
        public string RiskLevel { get; set; }
        public string Durum { get; set; }
    }

    public class RiskAnalizService
    {
        private const string ServerUrl = "http://127.0.0.1:5058/api/risk/analyze";

        public async Task<RiskAnalizVerisi> RiskAnaliziYap(
            long measureId,
            double pm25Value,
            double co2Value,
            double temperature,
            double humidity)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Add("User-Agent", "HavaDurumu-ASP.NET-MVC");

                    var requestData = new
                    {
                        measureId = measureId,
                        pm25Value = pm25Value,
                        co2Value = co2Value,
                        temperature = temperature,
                        humidity = humidity
                    };

                    var json = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(ServerUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<dynamic>(responseJson);

                        return new RiskAnalizVerisi
                        {
                            IsRisky = result.isRisky ?? false,
                            Message = result.message ?? "Risk analizi tamamlandi",
                            RiskLevel = result.riskLevel ?? "NORMAL",
                            Durum = "Basarili"
                        };
                    }
                    else
                    {
                        return new RiskAnalizVerisi
                        {
                            IsRisky = false,
                            Message = $"Sunucu hatasi: {response.StatusCode}",
                            RiskLevel = "Bilinmiyor",
                            Durum = "Hata"
                        };
                    }
                }
                catch (TaskCanceledException)
                {
                    return new RiskAnalizVerisi
                    {
                        IsRisky = false,
                        Message = "Risk analiz servisine baglanamadi (Timeout)",
                        RiskLevel = "Bilinmiyor",
                        Durum = "Baglanti Yok"
                    };
                }
                catch (HttpRequestException ex)
                {
                    return new RiskAnalizVerisi
                    {
                        IsRisky = false,
                        Message = $"Risk analiz servisine baglanamadi: {ex.Message}",
                        RiskLevel = "Bilinmiyor",
                        Durum = "Baglanti Yok"
                    };
                }
                catch (Exception ex)
                {
                    return new RiskAnalizVerisi
                    {
                        IsRisky = false,
                        Message = $"Risk analizi hatasi: {ex.Message}",
                        RiskLevel = "Bilinmiyor",
                        Durum = "Hata"
                    };
                }
            }
        }
    }
}
