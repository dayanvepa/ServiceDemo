using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceDemo.Domain.Interfaces
{
    public interface IAbstractRepository<TEntity, TKey> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(TKey id);
        Task<bool> ExistsAsync(TKey id);
    }
}