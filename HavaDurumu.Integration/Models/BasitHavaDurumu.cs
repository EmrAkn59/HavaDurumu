using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HavaDurumu.Integration
{
    // 1. Veriyi karşılayacak basit kalıp (Model)
    public class HavaDurumuVerisi
    {
        public CurrentWeather current_weather { get; set; }
    }

    public class CurrentWeather
    {
        public double temperature { get; set; } // Sıcaklık
        public double windspeed { get; set; }   // Rüzgar hızı
        public double winddirection { get; set; } // Rüzgar yönü
        public int weathercode { get; set; } // Hava durumu kodu
        public string time { get; set; } // Zaman
    }

    // Alternatif API için model (wttr.in)
    public class WttrInResponse
    {
        public Current[] current_condition { get; set; }
    }

    public class Current
    {
        public string temp_C { get; set; }
        public string windspeedKmph { get; set; }
        public string winddirDegree { get; set; }
        public string weatherCode { get; set; }
        public string localObsDateTime { get; set; }
    }

    // 2. Veriyi çeken basit servis (API İşlemi)
    public class HavaDurumuServisi
    {
        // Artık dışarıdan enlem (lat) ve boylam (lon) alıyor
        public async Task<HavaDurumuVerisi> VeriyiGetir(double lat, double lon)
        {
            // Önce Open-Meteo API'sini dene
            HavaDurumuVerisi result = await VeriyiGetirOpenMeteo(lat, lon);
            if (result != null && result.current_weather != null)
            {
                return result;
            }

            // Open-Meteo başarısız olursa, alternatif API'yi dene (wttr.in)
            return await VeriyiGetirWttrIn(lat, lon);
        }

        // Birincil API: Open-Meteo
        private async Task<HavaDurumuVerisi> VeriyiGetirOpenMeteo(double lat, double lon)
        {
            try
            {
                string url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    client.Timeout = TimeSpan.FromSeconds(10); // 10 saniye timeout
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<HavaDurumuVerisi>(json);
                    }
                }
            }
            catch
            {
                // Hata durumunda null döndür, alternatif API denenir
            }
            return null;
        }

        // Alternatif API: wttr.in
        private async Task<HavaDurumuVerisi> VeriyiGetirWttrIn(double lat, double lon)
        {
            try
            {
                // wttr.in API'si koordinatları kabul ediyor
                string url = $"https://wttr.in/{lat},{lon}?format=j1";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    client.Timeout = TimeSpan.FromSeconds(10);
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var wttrData = JsonConvert.DeserializeObject<WttrInResponse>(json);

                        if (wttrData != null && wttrData.current_condition != null && wttrData.current_condition.Length > 0)
                        {
                            var current = wttrData.current_condition[0];
                            
                            // wttr.in verisini HavaDurumuVerisi formatına dönüştür
                            return new HavaDurumuVerisi
                            {
                                current_weather = new CurrentWeather
                                {
                                    temperature = double.Parse(current.temp_C ?? "0"),
                                    windspeed = double.Parse(current.windspeedKmph ?? "0"),
                                    winddirection = double.Parse(current.winddirDegree ?? "0"),
                                    weathercode = int.Parse(current.weatherCode ?? "0"),
                                    time = current.localObsDateTime ?? DateTime.Now.ToString("yyyy-MM-ddTHH:mm")
                                }
                            };
                        }
                    }
                }
            }
            catch
            {
                // Hata durumunda null döndür
            }
            return null;
        }
    }
}