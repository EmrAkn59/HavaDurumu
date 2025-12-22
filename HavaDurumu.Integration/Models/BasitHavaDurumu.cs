using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HavaDurumu.Integration
{
    // 1. Veriyi karsilayacak basit kalip (Model)
    public class HavaDurumuVerisi
    {
        public CurrentWeather current_weather { get; set; }
    }

    public class CurrentWeather
    {
        public double temperature { get; set; } // Sicaklik
        public double windspeed { get; set; }   // Ruzgar hizi
        public double winddirection { get; set; } // Ruzgar yonu
        public int weathercode { get; set; } // Hava durumu kodu
        public string time { get; set; } // Zaman
    }

    // Alternatif API icin model (wttr.in)
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

    // 2. Veriyi ceken basit servis (API Islemi)
    public class HavaDurumuServisi
    {
        // Artik disaridan enlem (lat) ve boylam (lon) aliyor
        public async Task<HavaDurumuVerisi> VeriyiGetir(double lat, double lon)
        {
            // Once Open-Meteo API'sini dene
            HavaDurumuVerisi result = await VeriyiGetirOpenMeteo(lat, lon);
            if (result != null && result.current_weather != null)
            {
                return result;
            }

            // Open-Meteo basarisiz olursa, alternatif API'yi dene (wttr.in)
            return await VeriyiGetirWttrIn(lat, lon);
        }

        // Birincil API: Open-Meteo
        private async Task<HavaDurumuVerisi> VeriyiGetirOpenMeteo(double lat, double lon)
        {
            try
            {
                // InvariantCulture kullanarak . ile ondalik ayirici garantile
                string latStr = lat.ToString(CultureInfo.InvariantCulture);
                string lonStr = lon.ToString(CultureInfo.InvariantCulture);
                string url = $"https://api.open-meteo.com/v1/forecast?latitude={latStr}&longitude={lonStr}&current_weather=true";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                    client.Timeout = TimeSpan.FromSeconds(15); // 15 saniye timeout
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<HavaDurumuVerisi>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Open-Meteo API Hatasi: {ex.Message}");
            }
            return null;
        }

        // Alternatif API: wttr.in
        private async Task<HavaDurumuVerisi> VeriyiGetirWttrIn(double lat, double lon)
        {
            try
            {
                // InvariantCulture kullanarak . ile ondalik ayirici garantile
                string latStr = lat.ToString(CultureInfo.InvariantCulture);
                string lonStr = lon.ToString(CultureInfo.InvariantCulture);
                string url = $"https://wttr.in/{latStr},{lonStr}?format=j1";

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                    client.Timeout = TimeSpan.FromSeconds(15);
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var wttrData = JsonConvert.DeserializeObject<WttrInResponse>(json);

                        if (wttrData != null && wttrData.current_condition != null && wttrData.current_condition.Length > 0)
                        {
                            var current = wttrData.current_condition[0];
                            
                            // wttr.in verisini HavaDurumuVerisi formatina donustur
                            double temp = 0, wind = 0, windDir = 0;
                            int code = 0;
                            
                            double.TryParse(current.temp_C, NumberStyles.Any, CultureInfo.InvariantCulture, out temp);
                            double.TryParse(current.windspeedKmph, NumberStyles.Any, CultureInfo.InvariantCulture, out wind);
                            double.TryParse(current.winddirDegree, NumberStyles.Any, CultureInfo.InvariantCulture, out windDir);
                            int.TryParse(current.weatherCode, out code);

                            return new HavaDurumuVerisi
                            {
                                current_weather = new CurrentWeather
                                {
                                    temperature = temp,
                                    windspeed = wind,
                                    winddirection = windDir,
                                    weathercode = code,
                                    time = current.localObsDateTime ?? DateTime.Now.ToString("yyyy-MM-ddTHH:mm")
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"wttr.in API Hatasi: {ex.Message}");
            }
            return null;
        }
    }
}
