using System;
using System.Linq;
using System.Web.Mvc;
using HavaDurumu.Business.Abstract;
using HavaDurumu.Data.Context;
using HavaDurumu.DataAccess.EntityFramework;
using HavaDurumu.Business.Services;
using HavaDurumu.DataAccess.Interfaces;

namespace HavaDurumu.Web.Controllers
{
    public class LoginController : Controller
    {
        IUserService _userService;

        public LoginController()
        {
            // Dependency Injection yerine manuel oluşturma (şimdilik)
            var context = new AppDbContext();
            IUserDal userDal = new EfUserDal(context);
            _userService = new UserManager(userDal);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // 2. Butona basılınca çalışacak kısım (POST)
        [HttpPost]
        public ActionResult GirisYap(string kadi, string sifre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(kadi) || string.IsNullOrWhiteSpace(sifre))
                {
                    ViewBag.Hata = "Kullanıcı adı ve şifre boş olamaz!";
                    return View("Index");
                }

                // Entity Framework ile giriş kontrolü
                // kadi aslında email olarak kullanılıyor
                var user = _userService.LoginUser(kadi, sifre);

                if (user != null) 
                {
                    Session["User"] = user.Email;
                    Session["UserID"] = user.UserID;
                    Session["FullName"] = user.FullName;

                    // Anasayfaya yönlendir (HomeController'ın Index'ine)
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // -- Giriş Başarısız --
                    ViewBag.Hata = "Email veya şifre hatalı!";
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Hata = "Bir hata oluştu: " + ex.Message;
                if (ex.InnerException != null)
                {
                    ViewBag.Hata += " | Detay: " + ex.InnerException.Message;
                }
                return View("Index");
            }
        }

        // Çıkış yap butonu için
        public ActionResult CikisYap()
        {
            Session.Clear(); // Oturumu sil
            return RedirectToAction("Index", "Login"); // Login sayfasına geri at
        }
    }
}