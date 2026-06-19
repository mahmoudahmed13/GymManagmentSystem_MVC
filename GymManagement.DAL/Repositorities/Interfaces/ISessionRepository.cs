using GymManagement.DAL.Data.Models;

namespace GymManagement.DAL.Repositorities.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionWithTrainerAndCategory(CancellationToken ct =  default);

        Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default);
    }
}
