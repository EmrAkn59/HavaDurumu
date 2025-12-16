using HavaDurumu.Data.Entities;
using HavaDurumu.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using HavaDurumu.Business.Abstract;

namespace HavaDurumu.Business.Services
{
    public class UserManager : IUserService
    {
        IUserDal _userDal;
        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        // --- GİRİŞ KONTROL METODU ---
        public User LoginUser(string email, string password)
        {
            // Veritabanında bu mail VE bu şifreye sahip biri var mı?
            // Varsa o kullanıcıyı getir, yoksa null getir.
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            // Email format kontrolü
            if (!IsValidEmail(email))
            {
                return null;
            }

            return _userDal.GetListByFilter(x => x.Email == email && x.PasswordHash == password).FirstOrDefault();
        }

        // --- CREATE (Ekleme) ---
        public void UserAdd(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Kullanıcı bilgisi boş olamaz.");
            }

            // Zorunlu alan kontrolleri
            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                throw new ArgumentException("Ad Soyad alanı zorunludur.", nameof(user));
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("Email alanı zorunludur.", nameof(user));
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new ArgumentException("Şifre alanı zorunludur.", nameof(user));
            }

            // Email format kontrolü
            if (!IsValidEmail(user.Email))
            {
                throw new ArgumentException("Geçerli bir email adresi giriniz.", nameof(user));
            }

            // Email uzunluk kontrolü
            if (user.Email.Length > 100)
            {
                throw new ArgumentException("Email adresi en fazla 100 karakter olabilir.", nameof(user));
            }

            // Şifre uzunluk kontrolü
            if (user.PasswordHash.Length < 6)
            {
                throw new ArgumentException("Şifre en az 6 karakter olmalıdır.", nameof(user));
            }

            if (user.PasswordHash.Length > 256)
            {
                throw new ArgumentException("Şifre en fazla 256 karakter olabilir.", nameof(user));
            }

            // FullName uzunluk kontrolü
            if (user.FullName.Length > 100)
            {
                throw new ArgumentException("Ad Soyad en fazla 100 karakter olabilir.", nameof(user));
            }

            // Email unique kontrolü (Aynı email'de kullanıcı var mı?)
            var existingUser = _userDal.GetListByFilter(x => x.Email == user.Email).FirstOrDefault();
            if (existingUser != null)
            {
                throw new InvalidOperationException("Bu email adresi zaten kullanılıyor.");
            }

            // RoleID kontrolü (0'dan büyük olmalı)
            if (user.RoleID <= 0)
            {
                throw new ArgumentException("Geçerli bir rol seçiniz.", nameof(user));
            }

            // CreatedAt otomatik set et
            if (user.CreatedAt == default(DateTime))
            {
                user.CreatedAt = DateTime.Now;
            }

            _userDal.Insert(user);
        }

        // --- READ (Okuma) ---
        public User GetById(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Geçerli bir kullanıcı ID'si giriniz.", nameof(id));
            }

            return _userDal.GetById(id);
        }

        public List<User> GetList()
        {
            return _userDal.GetList();
        }

        // --- UPDATE (Güncelleme) ---
        public void UserUpdate(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Kullanıcı bilgisi boş olamaz.");
            }

            if (user.UserID <= 0)
            {
                throw new ArgumentException("Geçerli bir kullanıcı ID'si giriniz.", nameof(user));
            }

            // Mevcut kullanıcıyı kontrol et
            var existingUser = _userDal.GetById(user.UserID);
            if (existingUser == null)
            {
                throw new InvalidOperationException("Güncellenecek kullanıcı bulunamadı.");
            }

            // Zorunlu alan kontrolleri
            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                throw new ArgumentException("Ad Soyad alanı zorunludur.", nameof(user));
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                throw new ArgumentException("Email alanı zorunludur.", nameof(user));
            }

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new ArgumentException("Şifre alanı zorunludur.", nameof(user));
            }

            // Email format kontrolü
            if (!IsValidEmail(user.Email))
            {
                throw new ArgumentException("Geçerli bir email adresi giriniz.", nameof(user));
            }

            // Email unique kontrolü (Kendi email'i hariç)
            var userWithSameEmail = _userDal.GetListByFilter(x => x.Email == user.Email && x.UserID != user.UserID).FirstOrDefault();
            if (userWithSameEmail != null)
            {
                throw new InvalidOperationException("Bu email adresi başka bir kullanıcı tarafından kullanılıyor.");
            }

            // Email uzunluk kontrolü
            if (user.Email.Length > 100)
            {
                throw new ArgumentException("Email adresi en fazla 100 karakter olabilir.", nameof(user));
            }

            // Şifre uzunluk kontrolü
            if (user.PasswordHash.Length < 6)
            {
                throw new ArgumentException("Şifre en az 6 karakter olmalıdır.", nameof(user));
            }

            if (user.PasswordHash.Length > 256)
            {
                throw new ArgumentException("Şifre en fazla 256 karakter olabilir.", nameof(user));
            }

            // FullName uzunluk kontrolü
            if (user.FullName.Length > 100)
            {
                throw new ArgumentException("Ad Soyad en fazla 100 karakter olabilir.", nameof(user));
            }

            // RoleID kontrolü
            if (user.RoleID <= 0)
            {
                throw new ArgumentException("Geçerli bir rol seçiniz.", nameof(user));
            }

            _userDal.Update(user);
        }

        // --- DELETE (Silme) ---
        public void UserDelete(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Kullanıcı bilgisi boş olamaz.");
            }

            if (user.UserID <= 0)
            {
                throw new ArgumentException("Geçerli bir kullanıcı ID'si giriniz.", nameof(user));
            }

            // Mevcut kullanıcıyı kontrol et
            var existingUser = _userDal.GetById(user.UserID);
            if (existingUser == null)
            {
                throw new InvalidOperationException("Silinecek kullanıcı bulunamadı.");
            }

            _userDal.Delete(user);
        }

        // --- YARDIMCI METODLAR ---
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Basit email format kontrolü
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }
    }
}
