# HavaDurumu.Integration

Bu katman, SOAP (WCF) protokolü ile dış sistemlerle iletişim sağlamak için oluşturulmuştur.

## Servis Endpoint

WCF servisi şu adreste yayınlanmaktadır:
- **URL**: `http://localhost:54568/Services/AirQualityService.svc`
- **WSDL**: `http://localhost:54568/Services/AirQualityService.svc?wsdl`

## Kullanılabilir Metodlar

### 1. GetAllActiveStations()
Tüm aktif istasyonları getirir.

**Dönüş Tipi**: `List<StationInfo>`

### 2. GetStationById(int stationId)
Belirli bir istasyonun bilgilerini getirir.

**Parametreler**:
- `stationId`: İstasyon ID'si

**Dönüş Tipi**: `StationInfo`

### 3. GetStationMeasurements(int stationId, int count)
Belirli bir istasyonun son ölçümlerini getirir.

**Parametreler**:
- `stationId`: İstasyon ID'si
- `count`: Getirilecek ölçüm sayısı

**Dönüş Tipi**: `List<MeasurementInfo>`

### 4. GetCityReport(string cityName, DateTime startDate, DateTime endDate)
Belirli bir şehir için rapor getirir (Stored Procedure kullanarak).

**Parametreler**:
- `cityName`: Şehir adı
- `startDate`: Başlangıç tarihi
- `endDate`: Bitiş tarihi

**Dönüş Tipi**: `CityReportInfo`

### 5. CalculateAQI(decimal pm25)
PM2.5 değerine göre AQI (Air Quality Index) hesaplar (Fonksiyon kullanarak).

**Parametreler**:
- `pm25`: PM2.5 değeri

**Dönüş Tipi**: `int` (1-4 arası: 1=İyi, 2=Orta, 3=Hassas, 4=Tehlikeli)

### 6. GetActiveAlertCount(int stationId)
Belirli bir istasyon için aktif alarm sayısını getirir (Fonksiyon kullanarak).

**Parametreler**:
- `stationId`: İstasyon ID'si

**Dönüş Tipi**: `int`

### 7. AddMeasurement(int stationId, decimal pm25, decimal co2, decimal temp, decimal humidity)
Yeni ölçüm ekler (Stored Procedure kullanarak). PM2.5 > 150 ise otomatik alarm oluşturur.

**Parametreler**:
- `stationId`: İstasyon ID'si
- `pm25`: PM2.5 değeri
- `co2`: CO2 değeri
- `temp`: Sıcaklık
- `humidity`: Nem

**Dönüş Tipi**: `bool` (Başarılı ise true)

## Test Etme

Servisi test etmek için:
1. Projeyi çalıştırın
2. Tarayıcıda `http://localhost:54568/Services/AirQualityService.svc` adresine gidin
3. WSDL'i görmek için `?wsdl` parametresini ekleyin
4. SOAP UI veya Postman gibi araçlarla test edebilirsiniz

## SOAP Request Örneği

```xml
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetAllActiveStations xmlns="http://havadurumu.com/soap" />
  </soap:Body>
</soap:Envelope>
```

