using HavaDurumu.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Business.Abstract
{
    public interface IUserService
    {
        // Standart Metodlar
        void UserAdd(User user);
        void UserDelete(User user);
        void UserUpdate(User user);
        List<User> GetList();
        User GetById(int id);

        // ÖZEL METOD: Giriş Yapma İşlemi
        User LoginUser(string email, string password);
    }
}
