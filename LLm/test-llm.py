#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
LLM Sunucusu Test Scripti
"""
import requests
import json
import sys

def test_llm_server():
    """LLM sunucusunu test et"""
    base_url = "http://127.0.0.1:5000"
    
    print("=" * 50)
    print("LLM Sunucusu Test Ediliyor...")
    print("=" * 50)
    print()
    
    # 1. Ana sayfa testi
    print("1. Ana sayfa testi (GET /)...")
    try:
        response = requests.get(base_url, timeout=5)
        print(f"   Status Code: {response.status_code}")
        print(f"   Response: {response.text[:200]}")
        print("   ✓ Ana sayfa çalışıyor")
    except requests.exceptions.ConnectionError:
        print("   ✗ HATA: Sunucuya bağlanılamadı. Sunucu çalışıyor mu?")
        print("   Çözüm: LLm klasöründe 'start-llm-server.bat' dosyasını çalıştırın.")
        return False
    except Exception as e:
        print(f"   ✗ HATA: {e}")
        return False
    
    print()
    
    # 2. Tahmin endpoint testi
    print("2. Tahmin endpoint testi (POST /tahmin)...")
    test_data = {
        "sehir": "istanbul",
        "tarih": "2025-12-16 15:00",
        "nem": 65.0,
        "ruzgar": 12.0,
        "basinc": 1013.0,
        "yagis_var_mi": 0
    }
    
    try:
        response = requests.post(
            f"{base_url}/tahmin",
            json=test_data,
            timeout=10,
            headers={"Content-Type": "application/json"}
        )
        print(f"   Status Code: {response.status_code}")
        
        if response.status_code == 200:
            result = response.json()
            print(f"   ✓ Tahmin başarılı!")
            print(f"   Tahmin Edilen Sıcaklık: {result.get('tahmin_edilen_sicaklik', 'N/A')} °C")
            print(f"   Şehir: {result.get('sehir', 'N/A')}")
            print(f"   Tarih: {result.get('tarih', 'N/A')}")
            print(f"   Başarı: {result.get('basari', False)}")
            return True
        else:
            print(f"   ✗ HATA: Status code {response.status_code}")
            print(f"   Response: {response.text}")
            return False
            
    except requests.exceptions.ConnectionError:
        print("   ✗ HATA: Sunucuya bağlanılamadı.")
        return False
    except Exception as e:
        print(f"   ✗ HATA: {e}")
        return False

if __name__ == "__main__":
    success = test_llm_server()
    print()
    print("=" * 50)
    if success:
        print("✓ Tüm testler başarılı!")
        sys.exit(0)
    else:
        print("✗ Bazı testler başarısız!")
        sys.exit(1)

