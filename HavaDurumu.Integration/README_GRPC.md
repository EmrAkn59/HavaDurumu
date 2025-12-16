# gRPC Integration - Hava Kalitesi Servisi

Bu katmanda gRPC protokolü ile hava kalitesi değerlendirmesi yapılmaktadır.

## Servis Özelliği

### GetAirQualityStatus

Kirlilik değerine (PM2.5) göre hava kalitesi durumunu döndürür.

**Request:**
```protobuf
message PollutionRequest {
  double pollution_value = 1; // PM2.5 değeri (örn: 150)
}
```

**Response:**
```protobuf
message AirQualityResponse {
  string message = 1; // Örn: "Hava Kalitesi: SAĞLIKSIZ"
  int32 aqi_level = 2; // AQI seviyesi (1-4)
}
```

### AQI Seviyeleri

- **1**: İYİ (PM2.5 < 50)
- **2**: ORTA (PM2.5: 50-100)
- **3**: HASSAS (PM2.5: 100-150)
- **4**: SAĞLIKSIZ (PM2.5 >= 150)

### Örnek Kullanım

**Request:**
- `pollution_value`: 150

**Response:**
- `message`: "Hava Kalitesi: SAĞLIKSIZ"
- `aqi_level`: 4

## Servis Host

gRPC servisini başlatmak için:

```csharp
var host = new GrpcServiceHost();
host.Start(); // Port 50051'de dinler
host.WaitForShutdown();
```

## Port

Varsayılan port: **50051**

## NuGet Paketleri

- `Grpc.Core` (2.46.6)
- `Grpc.Tools` (2.46.6) - Development dependency
- `Google.Protobuf` (3.21.12)

## Test Etme

gRPC servisini test etmek için:

1. Projeyi çalıştırın (`HavaDurumu.Integration`)
2. gRPC client ile `localhost:50051` adresine bağlanın
3. `GetAirQualityStatus` metodunu çağırın
4. Örnek: `pollution_value = 150` gönderin
5. Yanıt: `"Hava Kalitesi: SAĞLIKSIZ"` alın

## Notlar

.NET Framework 4.7.2'de gRPC desteği sınırlıdır. Tam gRPC desteği için .NET Core/.NET 5+ önerilir.
