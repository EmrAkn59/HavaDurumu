using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.Core.Abstract
{
    public interface IGenericDal<T> where T : class
    {
        // CRUD İşlemleri (Standart Menü)
        void Insert(T t);
        void Delete(T t);
        void Update(T t);
        List<T> GetList(); // Hepsini getir
        T GetById(int id); // ID'ye göre getir

        // Filtreli getirme (Örn: Sadece İstanbul'daki istasyonları getir)
        List<T> GetListByFilter(Expression<Func<T, bool>> filter);
    }
}
