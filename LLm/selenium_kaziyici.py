from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options
from webdriver_manager.chrome import ChromeDriverManager
import pandas as pd
import json
import re
import time

SEHIR_URL = "https://aqicn.org/city/istanbul/"
DOSYA_ADI = "selenium_gecmis_veri.csv"


def zorla_kazi():
    print(f"🕵️‍♀️ Selenium Ajanı Devrede! {SEHIR_URL} adresine gidiliyor...")

    #Tarayıcı Ayarları (Gizli Modda Çalışsın)
    chrome_options = Options()
    chrome_options.add_argument("--no-sandbox")
    chrome_options.add_argument("--disable-dev-shm-usage")
    # Gerçek kullanıcı gibi görünmek için:
    chrome_options.add_argument(
        "user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36")

    #Tarayıcıyı Başlat
    driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()), options=chrome_options)

    try:
        driver.get(SEHIR_URL)

        print("Sayfa yükleniyor, 5 saniye bekleniyor...")
        time.sleep(5)

        # Sayfanın tüm HTML kaynağını al
        html_icerik = driver.page_source

        # --- AYNI REGEX TAKTİĞİ ---
        # Ama bu sefer JavaScript çalışmış haldeki HTML'de arıyoruz
        match = re.search(r'historyV2\s*:\s*(\{.*?\})\s*,', html_icerik)

        if match:
            print("GİZLİ VERİ PAKETİ YAKALANDI!")
            json_metni = match.group(1)
            data = json.loads(json_metni)
            grafikler = data.get('graphs', {})

            parametreler = {
                'pm25': 'PM2.5',
                'pm10': 'PM10',
                'temp': 'Sıcaklık',
                'humidity': 'Nem',
                'wind': 'Rüzgar',
                'no2': 'NO2'
            }

            birlestirilmis_veri = {}

            for kod, isim in parametreler.items():
                veriler = grafikler.get(kod, [])
                print(f"📊 {isim}: {len(veriler)} kayıt bulundu.")

                for kayit in veriler:
                    zaman_ms = kayit[0]
                    deger = kayit[1]
                    tarih = pd.to_datetime(zaman_ms, unit='ms')

                    if tarih not in birlestirilmis_veri:
                        birlestirilmis_veri[tarih] = {"Tarih": tarih, "Sehir": "Istanbul"}

                    birlestirilmis_veri[tarih][isim] = deger

            # Kaydet
            final_liste = list(birlestirilmis_veri.values())
            df = pd.DataFrame(final_liste)

            # Sütunları düzenle
            if not df.empty:
                df.sort_values('Tarih', inplace=True)
                df.to_csv(DOSYA_ADI, index=False)
                print(f"🎉 BAŞARILI! Veriler '{DOSYA_ADI}' dosyasına indi.")
                print(df.head())
            else:
                print("JSON bulundu ama içi boş çıktı.")

        else:
            print("HATA: Gizli veri bu sayfada da bulunamadı. Site yapısı tamamen değişmiş olabilir.")
            # Hata ayıklama için HTML'i kaydedelim
            with open("hata_sayfasi.html", "w", encoding="utf-8") as f:
                f.write(html_icerik)
            print("Sayfa kaynağı 'hata_sayfasi.html' olarak kaydedildi, inceleyebilirsiniz.")

    except Exception as e:
        print(f"Kritik Hata: {e}")
    finally:
        driver.quit()


if __name__ == "__main__":
    zorla_kazi()