using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceDemo.Domain.Entities;

namespace ServiceDemo.Domain.Interfaces
{
    public interface IProductRepository : IAbstractRepository<Product, long>
    {
        Task<int> CountAsync();
        Task<IEnumerable<Product>> GetPagedAsync(int page, int pageSize);
    }
}