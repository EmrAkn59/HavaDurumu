from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from selenium.webdriver.chrome.options import Options
from webdriver_manager.chrome import ChromeDriverManager
from io import StringIO
import pandas as pd
import time
import random
from datetime import datetime, timedelta

# --- AYARLAR ---
DOSYA_ADI = "turkiye_hava_durumu_5sehir_FULL.csv"

# Hedef Şehirler ve Kodları
SEHIRLER = {
    "Istanbul": "tr/istanbul/LTBA",
    "Ankara": "tr/ankara/LTAC",
    "Izmir": "tr/izmir/LTBJ",
    "Antalya": "tr/antalya/LTAI",
    "Adana": "tr/adana/LTAF"
}

# Tarih Aralığı (1 Ocak 2024 - DÜN)
BASLANGIC_TARIHI = "2024-01-01"
dun = datetime.now() - timedelta(days=1)
BITIS_TARIHI = dun.strftime("%Y-%m-%d")


def get_driver():
    chrome_options = Options()
    chrome_options.add_argument("--headless=new")
    chrome_options.add_argument("--disable-gpu")
    chrome_options.add_argument("--no-sandbox")
    chrome_options.add_argument("--disable-dev-shm-usage")

    service = Service(ChromeDriverManager().install())
    return webdriver.Chrome(service=service, options=chrome_options)


def hava_durumu_ajan():
    tarihler = pd.date_range(start=BASLANGIC_TARIHI, end=BITIS_TARIHI)
    print(f"🕵️‍♀️ SÜPER AJAN Devrede!")
    print(f"🌍 Hedef: {len(SEHIRLER)} Şehir")
    print(f"📅 Tarih: {len(tarihler)} Gün (Toplam ~{len(SEHIRLER) * len(tarihler)} sayfa taranacak)")
    print("-" * 60)

    driver = get_driver()
    tum_veriler = []

    # --- ŞEHİR DÖNGÜSÜ ---
    for sehir_adi, url_kodu in SEHIRLER.items():
        print(f"\n🏙️  ŞEHİR: {sehir_adi.upper()} Başlıyor...")

        # --- GÜN DÖNGÜSÜ ---
        for index, tarih in enumerate(tarihler):
            tarih_str = tarih.strftime("%Y-%m-%d")
            url = f"https://www.wunderground.com/history/daily/{url_kodu}/date/{tarih_str}"

            # Her 40 istekte bir tarayıcıyı yenile (RAM şişmesini önler)
            if index > 0 and index % 40 == 0:
                print("   ♻️ Tarayıcı tazeleniyor...")
                driver.quit()
                time.sleep(2)
                driver = get_driver()

            print(f"   [{index + 1}/{len(tarihler)}] {sehir_adi} - {tarih_str} ...", end="")

            basari = False
            deneme = 0

            while not basari and deneme < 2:
                try:
                    driver.get(url)
                    time.sleep(3.5)

                    html_kaynak = StringIO(driver.page_source)

                    try:
                        # Tabloları oku (lxml yerine bs4 kullanıyoruz)
                        tablolar = pd.read_html(html_kaynak, flavor='bs4')
                    except ValueError:
                        tablolar = []

                    tablo_bulundu = False
                    for tablo in tablolar:
                        cols = str(tablo.columns)
                        # Doğru tablo kontrolü (İçinde Time, Temp, Wind olmalı)
                        if "Time" in cols and "Temperature" in cols and "Wind" in cols:
                            if len(tablo) > 5:  # En az 5 satır veri olmalı
                                tablo['Tarih_Gun'] = tarih_str
                                tablo['Sehir'] = sehir_adi
                                tum_veriler.append(tablo)
                                print(f" ✅ ({len(tablo)} satır)")
                                tablo_bulundu = True
                                basari = True
                                break

                    if not tablo_bulundu:
                        print(" ⚠️ Boş.", end="")
                        deneme += 1
                        time.sleep(2)

                except Exception:
                    print(" ❌ Hata.", end="")
                    try:
                        driver.quit()
                        driver = get_driver()
                    except:
                        pass
                    deneme += 1

            # Alt satıra geç
            if not basari:
                print(" (Atlandı)")

            # Hızlı gidip ban yemeyelim
            time.sleep(1)

    driver.quit()

    # --- KAYDETME ---
    if tum_veriler:
        print("\n🔄 Veriler birleştiriliyor...")
        final_df = pd.concat(tum_veriler, ignore_index=True)
        final_df.to_csv(DOSYA_ADI, index=False)
        print(f"🎉 TAMAMLANDI!")
        print(f"📂 Dosya: {DOSYA_ADI}")
        print(f"📊 Toplam Satır: {len(final_df)}")
    else:
        print("⚠️ Hiç veri çekilemedi.")


if __name__ == "__main__":
    hava_durumu_ajan()