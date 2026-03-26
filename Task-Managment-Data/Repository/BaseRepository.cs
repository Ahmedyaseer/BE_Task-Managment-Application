using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Task_Managment_Data.Core;
using TMA_Core.Repository;

namespace Task_Managment_Data.Repository
{
    public class BaseRepository<T>(AppDbContext context) : IBaseRepository<T> where T : class
    {
        protected readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _dbSet.FindAsync([id], cancellationToken);

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Delete(T entity)
            => _dbSet.Remove(entity);

        public IQueryable<T> GetQueryable()
            => _dbSet.AsQueryable();

        public async Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(predicate, cancellationToken);

        public async Task<int> CountAsync(
            Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
            => predicate is null
                ? await _dbSet.CountAsync(cancellationToken)
                : await _dbSet.CountAsync(predicate, cancellationToken);

        public async Task<IReadOnlyList<T>> FindAsync(
            Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }
}
