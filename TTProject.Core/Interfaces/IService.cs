using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTProject.Core.Interfaces
{
    public interface IService<T> where T : class
    {
        Task AddAsync(T entity);
        Task<T> GetByIdAsync(object id);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
    }

}
