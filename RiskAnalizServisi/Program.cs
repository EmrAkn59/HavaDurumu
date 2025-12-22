using RiskAnalizServisi.Services;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Kestrel ayarları - hem HTTP/1.1 hem HTTP/2 destekle
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP endpoint (REST API için - HTTP/1.1)
    options.ListenLocalhost(5058, o => o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2);
});

var app = builder.Build();

// gRPC endpoint
app.MapGrpcService<RiskService>();

// HTTP REST endpoint (client uyumlulugu icin)
app.MapPost("/api/risk/analyze", async (HttpContext context) =>
{
    try
    {
        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();
        var request = JsonSerializer.Deserialize<RiskAnalyzeRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (request == null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Gecersiz istek" }));
            return;
        }

        // Risk analizi
        bool isRisky = false;
        string riskLevel = "NORMAL";
        var uyariMesajlari = new List<string>();

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

        var result = new { isRisky, message = finalMessage, riskLevel };

        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(result));
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
    }
});

app.MapGet("/", () => "Risk Analiz Servisi calisiyor! HTTP: POST /api/risk/analyze, gRPC: PollutionRisk.AnalyzeRisk");

app.Run();

// HTTP request modeli
public class RiskAnalyzeRequest
{
    public long MeasureId { get; set; }
    public double Pm25Value { get; set; }
    public double Co2Value { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
}
