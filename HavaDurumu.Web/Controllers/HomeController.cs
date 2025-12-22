using HavaDurumu.Integration;
using HavaDurumu.Business.Abstract;
using HavaDurumu.Business.Services;
using HavaDurumu.DataAccess.Interfaces;
using HavaDurumu.DataAccess.EntityFramework;
using HavaDurumu.Data.Context;
using HavaDurumu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;

namespace HavaDurumu.Web.Controllers
{
    public class HomeController : Controller
    {
        HavaDurumuServisi havaServis = new HavaDurumuServisi();
        NodeService nodeServis = new NodeService();
        LLMService llmServis = new LLMService();
        RiskAnalizService riskAnalizServis = new RiskAnalizService();
        AppDbContext _context;
        IStationService _stationService;
        IMeasurementService _measurementService;
        IReportService _reportService;

        public HomeController()
        {
            _context = new AppDbContext();
            var stationDal = new EfStationDal(_context);
            _stationService = new StationManager(stationDal);
            var measurementDal = new EfMeasurementDal(_context);
            _measurementService = new MeasurementManager(measurementDal);
            var reportDal = new EfReportDal(_context);
            _reportService = new ReportManager(reportDal, _context);
        }
        
        public async Task<ActionResult> Index(int? cityId = null, int? stationId = null)
        {
            // Eğer kullanıcı giriş yapmamışsa Login sayfasına yönlendir
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // TempData değerlerini ViewBag'e kopyala (TempData bir kez okununca silinir)
            if (TempData["ApiCalled"] != null)
            {
                ViewBag.ApiCalled = TempData["ApiCalled"];
                TempData.Keep("ApiCalled"); // TempData'yı koru
            }
            if (TempData["ApiTemperature"] != null)
            {
                ViewBag.ApiTemperature = TempData["ApiTemperature"];
                TempData.Keep("ApiTemperature");
            }
            if (TempData["ApiWindspeed"] != null)
            {
                ViewBag.ApiWindspeed = TempData["ApiWindspeed"];
                TempData.Keep("ApiWindspeed");
            }
            if (TempData["ApiWeathercode"] != null)
            {
                ViewBag.ApiWeathercode = TempData["ApiWeathercode"];
                TempData.Keep("ApiWeathercode");
            }
            if (TempData["ApiWinddirection"] != null)
            {
                ViewBag.ApiWinddirection = TempData["ApiWinddirection"];
                TempData.Keep("ApiWinddirection");
            }
            if (TempData["ApiTime"] != null)
            {
                ViewBag.ApiTime = TempData["ApiTime"];
                TempData.Keep("ApiTime");
            }
            if (TempData["ApiLatitude"] != null)
            {
                ViewBag.ApiLatitude = TempData["ApiLatitude"];
                TempData.Keep("ApiLatitude");
            }
            if (TempData["ApiLongitude"] != null)
            {
                ViewBag.ApiLongitude = TempData["ApiLongitude"];
                TempData.Keep("ApiLongitude");
            }
            if (TempData["ApiCallTime"] != null)
            {
                ViewBag.ApiCallTime = TempData["ApiCallTime"];
                TempData.Keep("ApiCallTime");
            }
            if (TempData["ApiSource"] != null)
            {
                ViewBag.ApiSource = TempData["ApiSource"];
                TempData.Keep("ApiSource");
            }
            if (TempData["UpdateError"] != null)
            {
                ViewBag.UpdateError = TempData["UpdateError"];
                TempData.Keep("UpdateError");
            }
            if (TempData["UpdateSuccess"] != null)
            {
                ViewBag.UpdateSuccess = TempData["UpdateSuccess"];
                TempData.Keep("UpdateSuccess");
            }
            if (TempData["UpdateWarning"] != null)
            {
                ViewBag.UpdateWarning = TempData["UpdateWarning"];
                TempData.Keep("UpdateWarning");
            }

            try
            {
                // Tüm şehirleri listele (dropdown için)
                var allCities = _context.Cities
                    .OrderBy(c => c.CityName)
                    .ToList();
                ViewBag.AllCities = allCities;

                // Seçilen şehre göre istasyonları filtrele
                IQueryable<Station> stationsQuery = _context.Stations
                    .Include("City")
                    .Where(s => s.IsActive);

                if (cityId.HasValue && cityId.Value > 0)
                {
                    stationsQuery = stationsQuery.Where(s => s.CityID == cityId.Value);
                    ViewBag.SelectedCityId = cityId.Value;
                }

                var filteredStations = stationsQuery
                    .OrderBy(s => s.StationName)
                    .ToList();
                ViewBag.FilteredStations = filteredStations;

                // Seçilen istasyonu çek
                Station station = null;
                if (stationId.HasValue && stationId.Value > 0)
                {
                    station = filteredStations.FirstOrDefault(s => s.StationID == stationId.Value);
                }

                // Eğer seçilen istasyon bulunamadıysa veya seçilmemişse, filtrelenmiş istasyonlardan ilkini al
                if (station == null && filteredStations.Count > 0)
                {
                    station = filteredStations.First();
                }
                else if (station == null)
                {
                    // Hiç istasyon yoksa, tüm aktif istasyonlardan ilkini al
                    station = _context.Stations
                        .Include("City")
                        .Where(s => s.IsActive)
                        .OrderBy(s => s.StationID)
                        .FirstOrDefault() ?? _context.Stations.Include("City").FirstOrDefault();
                }

                if (station != null)
                {
                    // İstasyon bilgileri
                    ViewBag.StationName = station.StationName;
                    ViewBag.CityName = station.City?.CityName ?? "Bilinmeyen";
                    ViewBag.IsActive = station.IsActive;
                    ViewBag.StationID = station.StationID;

                    // Node.js servisinden veri çek (son ölçümden PM2.5 değeri varsa gönder)
                    decimal? pm25Value = null;
                    var latestMeasurementForNode = _context.Measurements
                        .Where(m => m.StationID == station.StationID)
                        .OrderByDescending(m => m.MeasureDate)
                        .FirstOrDefault();
                    
                    if (latestMeasurementForNode != null && latestMeasurementForNode.PM25_Value.HasValue)
                    {
                        pm25Value = latestMeasurementForNode.PM25_Value.Value;
                    }

                    var nodeData = await nodeServis.NodeVerisiniGetir(pm25Value.HasValue ? (double?)pm25Value.Value : null);
                    ViewBag.NodeMesaj = nodeData?.mesaj ?? "API Baglantisi Yok";
                    ViewBag.NodeDurum = nodeData?.durum ?? "Baglanti Yok";
                    ViewBag.NodePM25 = nodeData?.pm25;
                    ViewBag.NodeSeviye = nodeData?.seviye;
                    ViewBag.NodeRenk = nodeData?.renk ?? "gray";
                    ViewBag.NodeTarih = nodeData?.tarih;

                    // En son ölçüm verisini çek
                    var latestMeasurement = _context.Measurements
                        .Where(m => m.StationID == station.StationID)
                        .OrderByDescending(m => m.MeasureDate)
                        .FirstOrDefault();

                    if (latestMeasurement != null)
                    {
                        ViewBag.Sicaklik = latestMeasurement.Temperature?.ToString("F1") ?? "0";
                        ViewBag.Nem = latestMeasurement.Humidity?.ToString("F1") ?? "0";
                        ViewBag.PM25 = latestMeasurement.PM25_Value?.ToString("F0") ?? "0";
                        ViewBag.CO2 = latestMeasurement.CO2_Value?.ToString("F0") ?? "0";
                        ViewBag.LastMeasureDate = latestMeasurement.MeasureDate;

                        // Risk Analiz Servisi'nden risk analizi yap
                        try
                        {
                            var riskAnaliz = await riskAnalizServis.RiskAnaliziYap(
                                latestMeasurement.MeasureID,
                                (double)(latestMeasurement.PM25_Value ?? 0),
                                (double)(latestMeasurement.CO2_Value ?? 0),
                                (double)(latestMeasurement.Temperature ?? 0),
                                (double)(latestMeasurement.Humidity ?? 0)
                            );

                            ViewBag.RiskIsRisky = riskAnaliz.IsRisky;
                            ViewBag.RiskMessage = riskAnaliz.Message;
                            ViewBag.RiskLevel = riskAnaliz.RiskLevel;
                            ViewBag.RiskDurum = riskAnaliz.Durum;
                        }
                        catch (Exception riskEx)
                        {
                            ViewBag.RiskIsRisky = false;
                            ViewBag.RiskMessage = $"Risk analizi yapılamadı: {riskEx.Message}";
                            ViewBag.RiskLevel = "Bilinmiyor";
                            ViewBag.RiskDurum = "Hata";
                        }
                    }
                    else
                    {
                        // Ölçüm yoksa varsayılan değerler
                        ViewBag.Sicaklik = "0";
                        ViewBag.Nem = "0";
                        ViewBag.PM25 = "0";
                        ViewBag.CO2 = "0";
                        ViewBag.LastMeasureDate = DateTime.Now;
                        ViewBag.RiskIsRisky = false;
                        ViewBag.RiskMessage = "Ölçüm verisi olmadığı için risk analizi yapılamadı.";
                        ViewBag.RiskLevel = "Bilinmiyor";
                        ViewBag.RiskDurum = "Veri Yok";
                    }

                    // Son 24 saatlik ölçüm verilerini çek (grafik için)
                    var last24Hours = DateTime.Now.AddHours(-24);
                    var measurements24h = _context.Measurements
                        .Where(m => m.StationID == station.StationID && m.MeasureDate >= last24Hours)
                        .OrderBy(m => m.MeasureDate)
                        .ToList();

                    // Grafik için veri hazırla (saatlik ortalamalar veya son 6 ölçüm)
                    var chartData = new List<object>();
                    var chartLabels = new List<string>();
                    
                    if (measurements24h.Count > 0)
                    {
                        // Son 6 ölçümü al veya saatlik gruplar oluştur
                        var recentMeasurements = measurements24h
                            .OrderByDescending(m => m.MeasureDate)
                            .Take(6)
                            .OrderBy(m => m.MeasureDate)
                            .ToList();

                        foreach (var m in recentMeasurements)
                        {
                            chartLabels.Add(m.MeasureDate.ToString("HH:mm"));
                            chartData.Add(m.Temperature ?? 0);
                        }
                    }
                    else
                    {
                        // Veri yoksa varsayılan değerler
                        chartLabels = new List<string> { "00:00", "04:00", "08:00", "12:00", "16:00", "20:00" };
                        chartData = new List<object> { 0, 0, 0, 0, 0, 0 };
                    }

                    ViewBag.ChartLabels = chartLabels;
                    ViewBag.ChartData = chartData;
                }
                else
                {
                    // İstasyon yoksa varsayılan değerler
                    ViewBag.StationName = "Istasyon Bulunamadi";
                    ViewBag.CityName = "Bilinmeyen";
                    ViewBag.Sicaklik = "0";
                    ViewBag.Nem = "0";
                    ViewBag.PM25 = "0";
                    ViewBag.CO2 = "0";
                    ViewBag.ChartLabels = new List<string> { "00:00", "04:00", "08:00", "12:00", "16:00", "20:00" };
                    ViewBag.ChartData = new List<object> { 0, 0, 0, 0, 0, 0 };

                    // Node.js servisinden veri çek (PM2.5 değeri olmadan)
                    var nodeData = await nodeServis.NodeVerisiniGetir(null);
                    ViewBag.NodeMesaj = nodeData?.mesaj ?? "API Baglantisi Yok";
                    ViewBag.NodeDurum = nodeData?.durum ?? "Baglanti Yok";
                    ViewBag.NodePM25 = nodeData?.pm25;
                    ViewBag.NodeSeviye = nodeData?.seviye;
                    ViewBag.NodeRenk = nodeData?.renk ?? "gray";
                    ViewBag.NodeTarih = nodeData?.tarih;
                }
            }
            catch (Exception ex)
            {
                ViewBag.NodeMesaj = "Hata: " + ex.Message;
                ViewBag.Sicaklik = "0";
                ViewBag.Nem = "0";
                ViewBag.PM25 = "0";
                ViewBag.CO2 = "0";
                ViewBag.ChartLabels = new List<string> { "00:00", "04:00", "08:00", "12:00", "16:00", "20:00" };
                ViewBag.ChartData = new List<object> { 0, 0, 0, 0, 0, 0 };
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateWeatherData(int? stationId)
        {
            try
            {
                if (!stationId.HasValue || stationId.Value <= 0)
                {
                    TempData["UpdateError"] = "Lütfen bir istasyon seçiniz.";
                    return RedirectToAction("Index");
                }

                // İstasyonu çek
                var station = _context.Stations
                    .Include("City")
                    .FirstOrDefault(s => s.StationID == stationId.Value && s.IsActive);

                if (station == null)
                {
                    TempData["UpdateError"] = "İstasyon bulunamadı.";
                    return RedirectToAction("Index", new { stationId = stationId });
                }

                // İstasyonun koordinatlarını kontrol et, yoksa varsayılan koordinatlar kullan (İstanbul)
                double lat, lon;
                if (station.Latitude.HasValue && station.Longitude.HasValue)
                {
                    lat = (double)station.Latitude.Value;
                    lon = (double)station.Longitude.Value;
                }
                else
                {
                    // Varsayılan koordinatlar (İstanbul)
                    lat = 41.0082;
                    lon = 28.9784;
                    TempData["UpdateWarning"] = "İstasyon koordinatları bulunamadı, İstanbul koordinatları kullanıldı.";
                }

                // HavaDurumuServisi'nden anlık veri çek (otomatik fallback ile)
                // VeriyiGetir metodu önce Open-Meteo'yu dener, başarısız olursa wttr.in'i dener
                HavaDurumuVerisi havaDurumuData = null;
                string apiSource = "Open-Meteo"; // Varsayılan API kaynağı
                string apiError = null;
                
                try
                {
                    havaDurumuData = await havaServis.VeriyiGetir(lat, lon);
                    
                    // Veri geldiyse, hangi API'den geldiğini belirle
                    // (VeriyiGetir içinde önce Open-Meteo denenir, başarısız olursa wttr.in denenir)
                    if (havaDurumuData != null && havaDurumuData.current_weather != null)
                    {
                        // Veri geldi, başarılı (hangi API'den geldiğini bilmiyoruz ama çalışıyor)
                        apiSource = "Open-Meteo veya wttr.in";
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    apiError = $"HTTP Hatası: {httpEx.Message}";
                    if (httpEx.InnerException != null)
                    {
                        apiError += $" | Detay: {httpEx.InnerException.Message}";
                    }
                }
                catch (Exception apiEx)
                {
                    apiError = $"API Hatası: {apiEx.Message}";
                    if (apiEx.InnerException != null)
                    {
                        apiError += $" | Detay: {apiEx.InnerException.Message}";
                    }
                }

                if (havaDurumuData == null || havaDurumuData.current_weather == null)
                {
                    TempData["UpdateError"] = "Tüm API kaynaklarından (Open-Meteo ve wttr.in) veri alınamadı. Lütfen daha sonra tekrar deneyin.";
                    TempData["ApiCalled"] = false;
                    return RedirectToAction("Index", new { stationId = stationId, cityId = station.CityID });
                }

                // API'den gelen veriyi sadece göster, veritabanına kaydetme
                TempData["UpdateSuccess"] = $"API'den veri başarıyla alındı ({apiSource}). Sıcaklık: {havaDurumuData.current_weather.temperature}°C";
                TempData["ApiTemperature"] = havaDurumuData.current_weather.temperature;
                TempData["ApiWindspeed"] = havaDurumuData.current_weather.windspeed;
                TempData["ApiWeathercode"] = havaDurumuData.current_weather.weathercode;
                TempData["ApiWinddirection"] = havaDurumuData.current_weather.winddirection;
                TempData["ApiTime"] = havaDurumuData.current_weather.time;
                TempData["ApiLatitude"] = lat;
                TempData["ApiLongitude"] = lon;
                TempData["ApiSource"] = apiSource; // Hangi API kullanıldığını sakla
                TempData["ApiCalled"] = true;
                TempData["ApiCallTime"] = DateTime.Now;

                return RedirectToAction("Index", new { stationId = stationId, cityId = station.CityID });
            }
            catch (Exception ex)
            {
                TempData["UpdateError"] = "Veri güncellenirken hata oluştu: " + ex.Message;
                return RedirectToAction("Index", new { stationId = stationId });
            }
        }

        [HttpPost]
        public async Task<JsonResult> LLMTahminYap(int? stationId = null)
        {
            try
            {
                if (stationId == null || stationId.Value <= 0)
                {
                    return Json(new { basari = false, hata = "İstasyon seçilmedi" });
                }

                var station = _context.Stations
                    .Include("City")
                    .FirstOrDefault(s => s.StationID == stationId.Value);

                if (station == null)
                {
                    return Json(new { basari = false, hata = "İstasyon bulunamadı" });
                }

                // En son ölçüm verisini çek
                var latestMeasurement = _context.Measurements
                    .Where(m => m.StationID == station.StationID)
                    .OrderByDescending(m => m.MeasureDate)
                    .FirstOrDefault();

                if (latestMeasurement == null)
                {
                    return Json(new { basari = false, hata = "Ölçüm verisi bulunamadı" });
                }

                // Şehir adını LLM'nin beklediği formata çevir
                string sehirAdi = station.City?.CityName?.ToLower() ?? "istanbul";
                if (sehirAdi.Contains("adana")) sehirAdi = "adana";
                else if (sehirAdi.Contains("ankara")) sehirAdi = "ankara";
                else if (sehirAdi.Contains("antalya")) sehirAdi = "antalya";
                else if (sehirAdi.Contains("istanbul")) sehirAdi = "istanbul";
                else if (sehirAdi.Contains("izmir")) sehirAdi = "izmir";
                else sehirAdi = "istanbul"; // Varsayılan

                string tarihStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                double nem = (double)(latestMeasurement.Humidity ?? 50);
                double ruzgar = 10; // Varsayılan (Measurement'da yok)
                double basinc = 1013; // Varsayılan (Measurement'da yok)
                int yagisVarMi = 0; // Varsayılan (yok)

                var llmTahmin = await llmServis.TahminYap(sehirAdi, tarihStr, nem, ruzgar, basinc, yagisVarMi);

                if (llmTahmin != null && llmTahmin.basari)
                {
                    return Json(new
                    {
                        basari = true,
                        tahmin_edilen_sicaklik = llmTahmin.tahmin_edilen_sicaklik?.ToString("F1") ?? "0",
                        sehir = llmTahmin.sehir,
                        tarih = llmTahmin.tarih
                    });
                }
                else
                {
                    return Json(new
                    {
                        basari = false,
                        hata = llmTahmin?.hata ?? "LLM tahmin yapılamadı"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    basari = false,
                    hata = $"LLM hatası: {ex.Message}"
                });
            }
        }
    }
}