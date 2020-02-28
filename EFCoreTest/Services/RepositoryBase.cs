using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EFCoreTest.Services
{
    /// <summary>
    /// 通用的仓储类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class RepositoryBase<T, TId> : IRepositoryBase<T>, IRepositoryBase2<T, TId>
        where T : class
    {
        public RepositoryBase(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public DbContext DbContext { get; }

        public void Create(T entity)
        {
            DbContext.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            DbContext.Set<T>().Remove(entity);
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(DbContext.Set<T>().AsEnumerable());
        }

        public Task<IEnumerable<T>> GetByContionAsync(Expression<Func<T, bool>> expression)
        {
            return Task.FromResult(DbContext.Set<T>().Where(expression).AsEnumerable());
        }

        public async Task<T> GetByIdAsync(TId id)
        {
            return await DbContext.Set<T>().FindAsync(id);
        }

        public async Task<bool> IsExistAsync(TId id)
        {
            return await DbContext.Set<T>().FindAsync(id) != null;
        }

        public async Task<bool> SaveAsync()
        {
            return await DbContext.SaveChangesAsync() > 0;
        }

        public void Update(T entity)
        {
            DbContext.Set<T>().Update(entity);
        }
    }
}
