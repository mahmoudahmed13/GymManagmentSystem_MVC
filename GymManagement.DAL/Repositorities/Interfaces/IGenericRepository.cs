using GymManagement.DAL.Data.Models;
using System.Linq.Expressions;

namespace GymManagement.DAL.Repositorities.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        Task<TEntity?> GetByIdAsync(int  id, CancellationToken ct = default);
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);
        Task<int> AddAsync(TEntity entity, CancellationToken ct = default);
        Task<int> UpdateAsync(TEntity entity, CancellationToken ct = default);
        Task<int> DeleteAsync(TEntity entity, CancellationToken ct = default);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate , CancellationToken ct = default);
    }
}
