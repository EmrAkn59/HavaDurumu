using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HavaDurumu.Integration
{
    // Node.js'ten gelecek verinin kalıbı
    public class NodeVerisi
    {
        public string mesaj { get; set; }
        public string tarih { get; set; }
        public string durum { get; set; }
        public double? pm25 { get; set; }
        public int? seviye { get; set; }
        public string renk { get; set; }
    }

    public class NodeService
    {
        // Node.js sunucusu 3000. portta çalışıyor
        private const string URL = "http://127.0.0.1:3000";

        public async Task<NodeVerisi> NodeVerisiniGetir(double? pm25 = null)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = URL;
                    if (pm25.HasValue)
                    {
                        url += $"?pm25={pm25.Value}";
                    }

                    // Timeout ayarla (10 saniye)
                    client.Timeout = TimeSpan.FromSeconds(10);
                    
                    // User-Agent ekle
                    client.DefaultRequestHeaders.Add("User-Agent", "HavaDurumu-ASP.NET-MVC");

                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<NodeVerisi>(json);
                        return result;
                    }
                    else
                    {
                        return new NodeVerisi 
                        { 
                            mesaj = $"Node.js sunucusu yanıt vermedi (Status: {response.StatusCode})", 
                            durum = "Hata" 
                        };
                    }
                }
                catch (TaskCanceledException)
                {
                    return new NodeVerisi 
                    { 
                        mesaj = "Node.js sunucusuna bağlanılamadı (Timeout)", 
                        durum = "Bağlantı Yok" 
                    };
                }
                catch (HttpRequestException ex)
                {
                    return new NodeVerisi 
                    { 
                        mesaj = $"Node.js sunucusuna bağlanılamadı: {ex.Message}", 
                        durum = "Bağlantı Yok" 
                    };
                }
                catch (Exception ex)
                {
                    return new NodeVerisi 
                    { 
                        mesaj = $"Node.js hatası: {ex.Message}", 
                        durum = "Hata" 
                    };
                }
            }
        }
    }
}