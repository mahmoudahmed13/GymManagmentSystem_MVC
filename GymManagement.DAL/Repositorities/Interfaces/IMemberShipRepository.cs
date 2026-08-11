using GymManagement.DAL.Data.Models;

namespace GymManagement.DAL.Repositorities.Interfaces
{
    public interface IMemberShipRepository : IGenericRepository<MemberShip>
    {
        Task<IEnumerable<MemberShip>> GetAllMemberShipWithMemberAndPlan(CancellationToken ct = default);
    }
}
