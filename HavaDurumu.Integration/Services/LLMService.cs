using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HavaDurumu.Integration
{
    // LLM'den gelecek tahmin verisinin kalıbı
    public class LLMTahminVerisi
    {
        public bool basari { get; set; }
        public double? tahmin_edilen_sicaklik { get; set; }
        public string sehir { get; set; }
        public string tarih { get; set; }
        public LLMGirdiler girdiler { get; set; }
        public string hata { get; set; }
    }

    public class LLMGirdiler
    {
        public double nem { get; set; }
        public double ruzgar { get; set; }
        public string yagis_durumu { get; set; }
    }

    public class LLMService
    {
        // Python Flask sunucusu 5000. portta çalışıyor
        private const string URL = "http://127.0.0.1:5000/tahmin";

        public async Task<LLMTahminVerisi> TahminYap(string sehir, string tarih, double nem, double ruzgar, double basinc, int yagisVarMi = 0)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Timeout ayarla (30 saniye - model tahmin yaparken biraz zaman alabilir)
                    client.Timeout = TimeSpan.FromSeconds(30);
                    
                    // User-Agent ekle
                    client.DefaultRequestHeaders.Add("User-Agent", "HavaDurumu-ASP.NET-MVC");

                    // JSON verisini hazırla
                    var requestData = new
                    {
                        sehir = sehir,
                        tarih = tarih,
                        nem = nem,
                        ruzgar = ruzgar,
                        basinc = basinc,
                        yagis_var_mi = yagisVarMi
                    };

                    var json = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(URL, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<LLMTahminVerisi>(responseJson);
                        return result;
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return new LLMTahminVerisi 
                        { 
                            basari = false,
                            hata = $"LLM sunucusu yanıt vermedi (Status: {response.StatusCode}). Detay: {errorContent}"
                        };
                    }
                }
                catch (TaskCanceledException)
                {
                    return new LLMTahminVerisi 
                    { 
                        basari = false,
                        hata = "LLM sunucusuna bağlanılamadı (Timeout). Python LLM sunucusunun çalıştığından emin olun (port 5000). LLm klasöründe 'start-llm-server.bat' dosyasını çalıştırın."
                    };
                }
                catch (HttpRequestException ex)
                {
                    string hataMesaji = ex.Message;
                    if (hataMesaji.Contains("No connection could be made") || hataMesaji.Contains("Connection refused"))
                    {
                        hataMesaji = "Python LLM sunucusu çalışmıyor. LLm klasöründe 'start-llm-server.bat' dosyasını çalıştırın (port 5000).";
                    }
                    return new LLMTahminVerisi 
                    { 
                        basari = false,
                        hata = $"LLM sunucusuna bağlanılamadı: {hataMesaji}"
                    };
                }
                catch (Exception ex)
                {
                    return new LLMTahminVerisi 
                    { 
                        basari = false,
                        hata = $"LLM hatası: {ex.Message}"
                    };
                }
            }
        }
    }
}

