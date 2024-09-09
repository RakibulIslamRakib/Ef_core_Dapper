using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Domain.Interfaces
{
   public interface IApplicationDbContext
   {
        public DbSet<T> Set<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
   }
}
