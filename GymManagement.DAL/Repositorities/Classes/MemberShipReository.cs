using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Repositorities.Classes
{
    public class MemberShipReository : GenericRepository<MemberShip>, IMemberShipRepository
    {
        private readonly GymDbContext _dbContext;

        public MemberShipReository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<MemberShip>> GetAllMemberShipWithMemberAndPlan(CancellationToken ct = default)
        {
            var memberShips = _dbContext.MemberShips.AsNoTracking()
                .Include(ms => ms.Member)
                .Include(ms => ms.Plan);
            return await memberShips.ToListAsync(ct);
        }
    }
}
