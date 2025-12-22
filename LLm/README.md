# Python LLM Sunucusu

Bu klasör Python Flask ile yazılmış bir LLM (Machine Learning Model) sunucusu içerir.

## Gereksinimler

- Python 3.x
- Flask
- Joblib
- Pandas
- NumPy

## Kurulum

1. Python'un yüklü olduğundan emin olun:
```bash
python --version
```

2. Gerekli paketleri yükleyin:
```bash
pip install flask joblib pandas numpy
```

## Kullanım

### Otomatik Başlatma (Önerilen)

`start-llm-server.bat` dosyasını çift tıklayın. Bu dosya:
- Python'un yüklü olup olmadığını kontrol eder
- Gerekli paketleri kontrol eder ve yükler
- Sunucuyu başlatır

### Manuel Başlatma

1. CMD veya PowerShell açın
2. LLm klasörüne gidin:
```bash
cd LLm
```

3. Sunucuyu başlatın:
```bash
python app.py
```

## Sunucu Bilgileri

- **Port:** 5000
- **URL:** http://127.0.0.1:5000
- **Ana Sayfa:** http://127.0.0.1:5000/
- **Tahmin Endpoint:** http://127.0.0.1:5000/tahmin (POST)

## Test Etme

Sunucu başladıktan sonra tarayıcıda şu adresi açın:
http://127.0.0.1:5000/

"✅ Model başarıyla yüklendi! Sunucu hazır." mesajını görmelisiniz.

## Sorun Giderme

### "Python bulunamadı" hatası
- Python'un PATH'e eklendiğinden emin olun
- Python'u yeniden yükleyin

### "Model yüklenemedi" hatası
- `final_model.pkl` dosyasının LLm klasöründe olduğundan emin olun

### Port 5000 kullanımda hatası
- Başka bir uygulama port 5000'i kullanıyor olabilir
- O uygulamayı kapatın veya `app.py` dosyasında port numarasını değiştirin

## Sunucuyu Durdurma

Sunucuyu durdurmak için CMD penceresinde `Ctrl+C` tuşlarına basın.

