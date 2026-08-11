using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GymManagement.DAL.Repositorities.Classes
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly GymDbContext _dbContext;

        public BookingRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default)
        {
            return _dbContext.Bookings.AsNoTracking().CountAsync(b => b.SessionId == sessionId);
        }

        public async Task<IEnumerable<Booking>> GetMembersForUpcomingSession(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Bookings.AsNoTracking()
                .Where(b => b.SessionId == sessionId && b.Session.StartDate > DateTime.UtcNow)
                .ToListAsync(ct);
        }
        public async Task<IEnumerable<Booking>> GetMembersForOngoingSession(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Bookings.AsNoTracking()
                .Where(b => b.SessionId == sessionId && b.Session.StartDate <= DateTime.UtcNow && b.Session.EndDate >= DateTime.UtcNow)
                .ToListAsync(ct);
        }

        public Task<Booking?> GetBookingByMemberAndSessionAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            return _dbContext.Bookings.AsNoTracking()
                .FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, ct);
        }
        public async Task<Member?> GetMemberWithMemberShip(int memberId, CancellationToken ct = default)
        {
            var member = await _dbContext.Members.AsNoTracking()
                .Include(m => m.MemberShips)
                .FirstOrDefaultAsync(m => m.Id == memberId, ct);
            return member;
        }

        public Task<Booking?> GetBookingWithSession(int sessionId, CancellationToken ct = default)
        {
            return _dbContext.Bookings.AsNoTracking()
                .Include(b => b.Session)
                .FirstOrDefaultAsync(b => b.SessionId == sessionId, ct);
        }
    }
}