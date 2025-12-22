from flask import Flask, request, jsonify
from flask_cors import CORS
import joblib
import pandas as pd
import numpy as np

app = Flask(__name__)
# CORS desteği ekle (tüm origin'lere izin ver)
CORS(app)
print("⏳ Model yükleniyor...")
try:
    model = joblib.load('final_model.pkl')
    print("✅ Model başarıyla yüklendi! Sunucu hazır.")
except Exception as e:
    print(f"❌ HATA: Model yüklenemedi! {e}")
    model = None

# Adana: 0, Ankara: 1, Antalya: 2, Istanbul: 3, Izmir: 4
SEHIR_KODLARI = {
    "adana": 0,
    "ankara": 1,
    "antalya": 2,
    "istanbul": 3,
    "izmir": 4
}

@app.route('/')
def home():
    model_durum = "✅ Model yüklendi" if model else "❌ Model yüklenemedi"
    return f"<h1>Hava Tahmin API Çalışıyor! 🚀</h1><p>Lütfen /tahmin adresine POST isteği atın.</p><p>Durum: {model_durum}</p>"

@app.route('/health', methods=['GET'])
def health_check():
    """Sunucu sağlık kontrolü"""
    if not model:
        return jsonify({'status': 'error', 'message': 'Model yüklenemedi'}), 500
    return jsonify({'status': 'ok', 'message': 'Sunucu çalışıyor', 'model_loaded': True}), 200

@app.route('/tahmin', methods=['POST'])
def tahmin_et():
    if not model:
        print("❌ HATA: Model yüklenmemiş!")
        return jsonify({'basari': False, 'hata': 'Model dosyası bulunamadı veya yüklenemedi.'}), 500

    try:
        # JSON verisini al
        veri = request.json
        if not veri:
            print("❌ HATA: JSON verisi alınamadı!")
            return jsonify({'basari': False, 'hata': 'JSON verisi bekleniyor.'}), 400
        print(f"📩 Gelen İstek: {veri}")

        # Gelen verileri değişkenlere ata (Varsayılan değerlerle)
        sehir_adi = veri.get('sehir', 'istanbul').lower()
        tarih_str = veri.get('tarih') # "2025-12-09 15:00"
        nem = float(veri.get('nem', 50))
        ruzgar = float(veri.get('ruzgar', 10))
        basinc = float(veri.get('basinc', 1013))
        yagis_var_mi = int(veri.get('yagis_var_mi', 0)) # 0: Yok, 1: Var

        # Şehir ismini koda çevir
        sehir_kodu = SEHIR_KODLARI.get(sehir_adi)
        if sehir_kodu is None:
            return jsonify({'hata': 'Geçersiz şehir ismi! (Sadece Adana, Ankara, Antalya, Istanbul, Izmir)'}), 400

        # Tarihi parçala
        try:
            tarih_obj = pd.to_datetime(tarih_str)
            ay = tarih_obj.month
            gun = tarih_obj.day
            saat = tarih_obj.hour
        except:
            return jsonify({'hata': 'Tarih formatı hatalı! YYYY-MM-DD HH:MM formatında olmalı.'}), 400

        # DataFrame oluştur (Modelin beklediği sütun sırasıyla!)
        # Sıra: ['Sehir_Kodu', 'Ay', 'Gun', 'Saat', 'Nem_Yuzde', 'Ruzgar_kmh', 'Basinc_hPa', 'Yagis_Var_Mi']
        girdi_df = pd.DataFrame([{
            'Sehir_Kodu': sehir_kodu,
            'Ay': ay,
            'Gun': gun,
            'Saat': saat,
            'Nem_Yuzde': nem,
            'Ruzgar_kmh': ruzgar,
            'Basinc_hPa': basinc,
            'Yagis_Var_Mi': yagis_var_mi
        }])

        # Tahmin yap
        sonuc = model.predict(girdi_df)[0]

        # Cevabı hazırla
        cevap = {
            'basari': True,
            'tahmin_edilen_sicaklik': round(sonuc, 2),
            'sehir': sehir_adi,
            'tarih': tarih_str,
            'girdiler': {
                'nem': nem,
                'ruzgar': ruzgar,
                'yagis_durumu': 'Var' if yagis_var_mi == 1 else 'Yok'
            }
        }
        print(f"✅ Tahmin başarılı: {round(sonuc, 2)}°C")
        return jsonify(cevap)

    except Exception as e:
        print(f"❌ HATA: {str(e)}")
        import traceback
        traceback.print_exc()
        return jsonify({'basari': False, 'hata': str(e)}), 400

if __name__ == '__main__':
    print("=" * 50)
    print("🚀 Flask Sunucusu Başlatılıyor...")
    print(f"📍 Port: 5000")
    print(f"🌐 URL: http://127.0.0.1:5000")
    print(f"📊 Model Durumu: {'✅ Yüklendi' if model else '❌ Yüklenemedi'}")
    print("=" * 50)
    print()
    try:
        app.run(debug=True, host='0.0.0.0', port=5000)
    except OSError as e:
        if "Address already in use" in str(e) or "address is already in use" in str(e).lower():
            print("❌ HATA: Port 5000 zaten kullanılıyor!")
            print("💡 Çözüm: Port 5000'i kullanan başka bir uygulamayı kapatın veya app.py'de port numarasını değiştirin.")
        else:
            print(f"❌ HATA: {e}")
        raise