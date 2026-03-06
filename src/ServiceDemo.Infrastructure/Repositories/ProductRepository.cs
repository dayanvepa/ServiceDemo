using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceDemo.Domain.Entities;
using ServiceDemo.Domain.Interfaces;
using ServiceDemo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace ServiceDemo.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ServiceDemoDbContext _context;

        public ProductRepository(ServiceDemoDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(long id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.Products
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public  async Task<int> CountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<Product> CreateAsync(Product entity)
        {
            await _context.Products.AddAsync(entity);
            return entity;
        }   

        public async Task<Product> UpdateAsync(Product entity)
        {
            _context.Products.Update(entity);
            return entity;
        }

        public async Task DeleteAsync(long id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
        }

        public async Task<bool> ExistsAsync(long id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

    }
}