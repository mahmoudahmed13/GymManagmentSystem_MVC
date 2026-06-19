using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.DAL.Repositorities.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _dbContext;
        private readonly Dictionary<string, object> _repositories = [];

        public UnitOfWork(GymDbContext dbContext, ISessionRepository sessionRepository)
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;
        }

        public ISessionRepository SessionRepository { get; }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            //CHeck TEntity ==??/ Plan,Member,Trainer..
            var typeName = typeof(TEntity).Name;
            //if Exists -> return
            if (_repositories.TryGetValue(typeName, out object? value))
                return (IGenericRepository<TEntity>)value;
            //if Not -> Create - Store - Return
            else
            {
                var repo = new GenericRepository<TEntity>(_dbContext);
                _repositories[typeName] = repo;
                return repo;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default) 
            => await _dbContext.SaveChangesAsync(ct);
    }
}
