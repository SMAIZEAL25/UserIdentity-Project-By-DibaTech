using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<AppUser> Users { get; }
        IGenericRepository<RefreshToken> RefreshTokens { get; }

        IGenericRepository<T> Repository <T>() where T : class;
        Task<int> SaveChangesAsync();
    }
}
