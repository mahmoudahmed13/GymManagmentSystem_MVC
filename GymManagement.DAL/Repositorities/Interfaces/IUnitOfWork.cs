using GymManagement.DAL.Data.Models;

namespace GymManagement.DAL.Repositorities.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        public ISessionRepository SessionRepository { get; }
    }
}
