using GymManagement.DAL.Data.Models;
using System.Linq.Expressions;

namespace GymManagement.DAL.Repositorities.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        Task<TEntity?> GetByIdAsync(int  id, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate , CancellationToken ct = default);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate , bool tracking = false , CancellationToken ct = default);
    }
}
