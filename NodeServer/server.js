const express = require('express');
const cors = require('cors');
const app = express();
const PORT = 3000;

// CORS - Tüm isteklere izin ver
app.use(cors());

// PM2.5 değerine göre hava kalitesi durumunu hesapla
function getAirQualityStatus(pm25) {
    if (pm25 <= 12) return { seviye: 1, durum: "İYİ", renk: "green" };
    if (pm25 <= 35) return { seviye: 2, durum: "ORTA", renk: "yellow" };
    if (pm25 <= 55) return { seviye: 3, durum: "HASSAS", renk: "orange" };
    return { seviye: 4, durum: "SAĞLIKSIZ", renk: "red" };
}

// Ana API endpoint - Hava kalitesi bilgisi
app.get('/', (req, res) => {
    const now = new Date();
    const pm25 = parseFloat(req.query.pm25) || 0; // Query parametresinden PM2.5 değeri al
    
    const airQuality = getAirQualityStatus(pm25);
    
    const response = {
        mesaj: `Hava Kalitesi: ${airQuality.durum}`,
        tarih: now.toLocaleString('tr-TR'),
        durum: airQuality.durum,
        pm25: pm25,
        seviye: airQuality.seviye,
        renk: airQuality.renk
    };
    
    res.json(response);
    console.log(`[${now.toLocaleTimeString('tr-TR')}] PM2.5: ${pm25} - Durum: ${airQuality.durum}`);
});

// Sunucuyu başlat
app.listen(PORT, '127.0.0.1', () => {
    console.log(`Node.js Hava Kalitesi API başlatıldı: http://127.0.0.1:${PORT}`);
    console.log(`Kullanım: http://127.0.0.1:${PORT}?pm25=25`);
});

