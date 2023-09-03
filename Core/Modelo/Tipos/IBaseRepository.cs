using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Tipos
{
    public interface IBaseRepository<T> where T : class, new()
    {
        Task<List<T>> GetItems();
        Task<T> UpdateItem(T item);
        Task<T> SaveItem(T item);
    }
}
