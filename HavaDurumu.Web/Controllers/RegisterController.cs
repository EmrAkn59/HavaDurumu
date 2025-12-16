using System;
using System.Linq;
using System.Web.Mvc;
using HavaDurumu.Business.Abstract;
using HavaDurumu.Data.Entities;
using HavaDurumu.DataAccess.Interfaces;
using HavaDurumu.Data.Context;
using HavaDurumu.DataAccess.EntityFramework;
using HavaDurumu.Business.Services;
using System.Data.Entity;

namespace HavaDurumu.Web.Controllers
{
    public class RegisterController : Controller
    {
        IUserService _userService;
        IUserDal _userDal;
        AppDbContext _context;

        public RegisterController()
        {
            // Dependency Injection yerine manuel oluşturma (şimdilik)
            _context = new AppDbContext();
            _userDal = new EfUserDal(_context);
            _userService = new UserManager(_userDal);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult KayitOl(string fullName, string email, string password, string confirmPassword, int roleID = 1)
        {
            try
            {
                // Validasyon kontrolleri
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    ViewBag.Hata = "Ad Soyad alanı zorunludur!";
                    return View("Index");
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    ViewBag.Hata = "Email alanı zorunludur!";
                    return View("Index");
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    ViewBag.Hata = "Şifre alanı zorunludur!";
                    return View("Index");
                }

                if (password.Length < 6)
                {
                    ViewBag.Hata = "Şifre en az 6 karakter olmalıdır!";
                    return View("Index");
                }

                if (password != confirmPassword)
                {
                    ViewBag.Hata = "Şifreler eşleşmiyor!";
                    return View("Index");
                }

                // Email format kontrolü
                if (!email.Contains("@") || !email.Contains("."))
                {
                    ViewBag.Hata = "Geçerli bir email adresi giriniz!";
                    return View("Index");
                }

                // RoleID kontrolü - Eğer RoleID=1 yoksa oluştur
                var role = _context.Roles.FirstOrDefault(r => r.RoleID == roleID);
                if (role == null)
                {
                    // Varsayılan rol yoksa oluştur
                    var defaultRole = new Role
                    {
                        RoleName = "Kullanıcı",
                        Description = "Normal kullanıcı rolü"
                    };
                    _context.Roles.Add(defaultRole);
                    _context.SaveChanges();
                    roleID = defaultRole.RoleID;
                }

                // Yeni kullanıcı oluştur
                var newUser = new User
                {
                    FullName = fullName,
                    Email = email,
                    PasswordHash = password, // Gerçek uygulamada hash'lenmeli
                    RoleID = roleID, // Varsayılan olarak 1 (Normal kullanıcı)
                    CreatedAt = DateTime.Now
                };

                // Kullanıcıyı ekle
                _userService.UserAdd(newUser);

                ViewBag.Basarili = "Kayıt başarılı! Giriş yapabilirsiniz.";
                return RedirectToAction("Index", "Login");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
            {
                // Inner exception'ı al
                var innerEx = dbEx.InnerException;
                string errorMessage = "Kayıt sırasında bir hata oluştu: " + dbEx.Message;
                
                if (innerEx != null)
                {
                    errorMessage += " | Detay: " + innerEx.Message;
                    
                    // SQL Server hatalarını kontrol et
                    if (innerEx is System.Data.SqlClient.SqlException sqlEx)
                    {
                        // Foreign key hatası
                        if (sqlEx.Number == 547)
                        {
                            errorMessage = "Hata: Seçilen rol geçersiz veya veritabanında bulunamadı. Lütfen sistem yöneticisi ile iletişime geçin.";
                        }
                        // Duplicate key hatası
                        else if (sqlEx.Number == 2627 || sqlEx.Number == 2601)
                        {
                            errorMessage = "Bu email adresi zaten kullanılıyor.";
                        }
                    }
                }
                
                ViewBag.Hata = errorMessage;
                return View("Index");
            }
            catch (Exception ex)
            {
                string errorMessage = "Kayıt sırasında bir hata oluştu: " + ex.Message;
                
                // Inner exception varsa ekle
                if (ex.InnerException != null)
                {
                    errorMessage += " | Detay: " + ex.InnerException.Message;
                }
                
                ViewBag.Hata = errorMessage;
                return View("Index");
            }
        }
    }
}

