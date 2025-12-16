using HavaDurumu.DataAccess.Abstract;
using HavaDurumu.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HavaDurumu.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericDal<T> where T : class
    {
        // Context nesnemizi oluşturuyoruz
        // Not: .NET Framework projelerinde context'i using içinde new'lemek güvenlidir.
        // Dependency Injection yapısı kuruluysa Constructor'dan da alınabilir.

        AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Delete(T t)
        {
            _context.Set<T>().Remove(t);
            _context.SaveChanges();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public List<T> GetList()
        {
            return _context.Set<T>().ToList();
        }

        public List<T> GetListByFilter(Expression<Func<T, bool>> filter)
        {
            return _context.Set<T>().Where(filter).ToList();
        }

        public void Insert(T t)
        {
            _context.Set<T>().Add(t);
            _context.SaveChanges();
        }

        public void Update(T t)
        {
            // Entity'nin context'te takip edilip edilmediğini kontrol et
            var entry = _context.Entry(t);
            
            // Eğer entity context'te takip edilmiyorsa (Detached state)
            if (entry.State == EntityState.Detached)
            {
                // Primary key'i bul (ID, UserID, StationID gibi)
                var keyProperty = GetKeyProperty(typeof(T));
                if (keyProperty != null)
                {
                    var keyValue = keyProperty.GetValue(t);
                    if (keyValue != null)
                    {
                        // Context'ten mevcut entity'yi al
                        var existingEntity = _context.Set<T>().Find(keyValue);
                        if (existingEntity != null)
                        {
                            // Mevcut entity'nin değerlerini güncelle
                            _context.Entry(existingEntity).CurrentValues.SetValues(t);
                            _context.SaveChanges();
                            return;
                        }
                    }
                }
                
                // Eğer mevcut entity bulunamadıysa, direkt attach et
                _context.Set<T>().Attach(t);
                entry.State = EntityState.Modified;
            }
            else
            {
                // Zaten context'te takip ediliyorsa direkt Modified yap
                entry.State = EntityState.Modified;
            }
            
            _context.SaveChanges();
        }

        // Primary key property'sini bul (ID, UserID, StationID gibi)
        private PropertyInfo GetKeyProperty(Type entityType)
        {
            // Önce "ID" property'sini ara
            var idProperty = entityType.GetProperty("ID");
            if (idProperty != null)
                return idProperty;

            // Sonra entity adı + "ID" formatını ara (örn: UserID, StationID)
            var entityName = entityType.Name;
            var keyPropertyName = entityName + "ID";
            var keyProperty = entityType.GetProperty(keyPropertyName);
            if (keyProperty != null)
                return keyProperty;

            // Key attribute'u olan property'yi ara
            var properties = entityType.GetProperties();
            foreach (var prop in properties)
            {
                var keyAttribute = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>();
                if (keyAttribute != null)
                    return prop;
            }

            return null;
        }
    }
}
