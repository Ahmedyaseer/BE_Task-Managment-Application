using Task_Managment_Data.Core;
using TMA_Core.Repository;

namespace Task_Managment_Data.Repository
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories = new();

        public IBaseRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
                _repositories[type] = new BaseRepository<T>(context);
            return (IBaseRepository<T>)_repositories[type];
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => context.SaveChangesAsync(cancellationToken);

        public void Dispose() => context.Dispose();
    }
}
