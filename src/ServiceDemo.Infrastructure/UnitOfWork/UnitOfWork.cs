using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceDemo.Domain.Interfaces;
using ServiceDemo.Infrastructure.Data;
using ServiceDemo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;


namespace ServiceDemo.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ServiceDemoDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ServiceDemoDbContext context)
        {
            _context = context;
            Products = new ProductRepository(_context);
        }
        public IProductRepository Products { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

    }
}