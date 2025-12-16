using HavaDurumu.Business.Abstract;
using HavaDurumu.DataAccess.Interfaces;
using HavaDurumu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Business.Services
{
    public class StationManager : IStationService
    {
        IStationDal _stationDal;

        public StationManager(IStationDal stationDal)
        {
            _stationDal = stationDal;
        }

        // --- CREATE (Ekleme) ---
        public void TAdd(Station t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t), "İstasyon bilgisi boş olamaz.");
            }

            // Zorunlu alan kontrolleri
            if (string.IsNullOrWhiteSpace(t.StationName))
            {
                throw new ArgumentException("İstasyon adı zorunludur.", nameof(t));
            }

            // StationName uzunluk kontrolü
            if (t.StationName.Length > 100)
            {
                throw new ArgumentException("İstasyon adı en fazla 100 karakter olabilir.", nameof(t));
            }

            // CityID kontrolü (0'dan büyük olmalı)
            if (t.CityID <= 0)
            {
                throw new ArgumentException("Geçerli bir şehir seçiniz.", nameof(t));
            }

            // Koordinat validasyonu (Latitude: -90 ile 90 arası)
            if (t.Latitude.HasValue)
            {
                if (t.Latitude.Value < -90 || t.Latitude.Value > 90)
                {
                    throw new ArgumentException("Enlem (Latitude) -90 ile 90 arasında olmalıdır.", nameof(t));
                }
            }

            // Koordinat validasyonu (Longitude: -180 ile 180 arası)
            if (t.Longitude.HasValue)
            {
                if (t.Longitude.Value < -180 || t.Longitude.Value > 180)
                {
                    throw new ArgumentException("Boylam (Longitude) -180 ile 180 arasında olmalıdır.", nameof(t));
                }
            }

            // İstasyon adı unique kontrolü (Aynı şehirde aynı isimde istasyon var mı?)
            var existingStation = _stationDal.GetListByFilter(x => x.StationName == t.StationName && x.CityID == t.CityID).FirstOrDefault();
            if (existingStation != null)
            {
                throw new InvalidOperationException("Bu şehirde aynı isimde bir istasyon zaten mevcut.");
            }

            // IsActive varsayılan değer (zaten default true)

            _stationDal.Insert(t);
        }

        // --- READ (Okuma) ---
        public Station TGetById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Geçerli bir istasyon ID'si giriniz.", nameof(id));
            }

            return _stationDal.GetById(id);
        }

        public List<Station> TGetList()
        {
            return _stationDal.GetList();
        }

        // --- UPDATE (Güncelleme) ---
        public void TUpdate(Station t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t), "İstasyon bilgisi boş olamaz.");
            }

            if (t.StationID <= 0)
            {
                throw new ArgumentException("Geçerli bir istasyon ID'si giriniz.", nameof(t));
            }

            // Mevcut istasyonu kontrol et
            var existingStation = _stationDal.GetById(t.StationID);
            if (existingStation == null)
            {
                throw new InvalidOperationException("Güncellenecek istasyon bulunamadı.");
            }

            // Zorunlu alan kontrolleri
            if (string.IsNullOrWhiteSpace(t.StationName))
            {
                throw new ArgumentException("İstasyon adı zorunludur.", nameof(t));
            }

            // StationName uzunluk kontrolü
            if (t.StationName.Length > 100)
            {
                throw new ArgumentException("İstasyon adı en fazla 100 karakter olabilir.", nameof(t));
            }

            // CityID kontrolü
            if (t.CityID <= 0)
            {
                throw new ArgumentException("Geçerli bir şehir seçiniz.", nameof(t));
            }

            // Koordinat validasyonu (Latitude: -90 ile 90 arası)
            if (t.Latitude.HasValue)
            {
                if (t.Latitude.Value < -90 || t.Latitude.Value > 90)
                {
                    throw new ArgumentException("Enlem (Latitude) -90 ile 90 arasında olmalıdır.", nameof(t));
                }
            }

            // Koordinat validasyonu (Longitude: -180 ile 180 arası)
            if (t.Longitude.HasValue)
            {
                if (t.Longitude.Value < -180 || t.Longitude.Value > 180)
                {
                    throw new ArgumentException("Boylam (Longitude) -180 ile 180 arasında olmalıdır.", nameof(t));
                }
            }

            // İstasyon adı unique kontrolü (Kendi istasyonu hariç, aynı şehirde)
            var stationWithSameName = _stationDal.GetListByFilter(x => x.StationName == t.StationName && x.CityID == t.CityID && x.StationID != t.StationID).FirstOrDefault();
            if (stationWithSameName != null)
            {
                throw new InvalidOperationException("Bu şehirde aynı isimde başka bir istasyon mevcut.");
            }

            _stationDal.Update(t);
        }

        // --- DELETE (Silme) ---
        public void TDelete(Station t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t), "İstasyon bilgisi boş olamaz.");
            }

            if (t.StationID <= 0)
            {
                throw new ArgumentException("Geçerli bir istasyon ID'si giriniz.", nameof(t));
            }

            // Mevcut istasyonu kontrol et
            var existingStation = _stationDal.GetById(t.StationID);
            if (existingStation == null)
            {
                throw new InvalidOperationException("Silinecek istasyon bulunamadı.");
            }

            // İlişkili kayıt kontrolü (Opsiyonel - eğer ölçümler varsa silme işlemini engelleyebilirsiniz)
            // Bu kontrolü yapmak için MeasurementDal'a ihtiyaç var, şimdilik sadece uyarı olarak bırakıyoruz
            // Not: Cascade delete ayarlanmışsa otomatik silinir

            _stationDal.Delete(t);
        }
    }
}
