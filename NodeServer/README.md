# Node.js API Sunucusu

Bu Node.js sunucusu, Hava Durumu ASP.NET MVC projesi için API servisi sağlar.

## Kurulum

1. Node.js'in yüklü olduğundan emin olun (v14 veya üzeri)
   ```bash
   node --version
   ```

2. Bağımlılıkları yükleyin:
   ```bash
   npm install
   ```

## Çalıştırma

Sunucuyu başlatmak için:

```bash
npm start
```

veya

```bash
node server.js
```

Sunucu `http://127.0.0.1:3000` adresinde çalışacaktır.

## API Endpoints

### GET /
Ana endpoint - Hava durumu verilerini döndürür.

**Yanıt:**
```json
{
  "mesaj": "Node.js sunucusu başarıyla çalışıyor!",
  "tarih": "14.12.2024 20:30:45",
  "durum": "Aktif"
}
```

### GET /health
Sunucu sağlık kontrolü.

**Yanıt:**
```json
{
  "status": "OK",
  "timestamp": "2024-12-14T20:30:45.123Z"
}
```

## Notlar

- Sunucu port 3000'de çalışır
- CORS etkinleştirilmiştir (ASP.NET MVC uygulamasından isteklere izin verir)
- Sunucuyu durdurmak için `Ctrl+C` tuşlarına basın

