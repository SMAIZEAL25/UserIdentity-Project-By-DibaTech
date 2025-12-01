using Application.Interface;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _DbContext;
        
        private readonly ConcurrentDictionary<Type, object> _repositories
           = new ConcurrentDictionary<Type, object>();     // Cache created repositories (Generic)
        public IGenericRepository<AppUser> Users { get; private set; }

        public IGenericRepository<RefreshToken> RefreshTokens { get; private set; }

        public UnitOfWork(AppDbContext DbContext)
        {
            _DbContext = DbContext;
            // Explicitly declared repositories
            Users = new GenericRepository<AppUser>(_DbContext);
            RefreshTokens = new GenericRepository<RefreshToken>(_DbContext);
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repository = new GenericRepository<T>(_DbContext);
                _repositories[type] = repository;
            }
            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _DbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _DbContext.Dispose();
        }

    }
}
