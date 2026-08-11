using GymManagement.DAL.Data.Models;

namespace GymManagement.DAL.Repositorities.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<int> GetCountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default);
        Task<IEnumerable<Booking>> GetMembersForOngoingSession(int sessionId, CancellationToken ct = default);
        Task<IEnumerable<Booking>> GetMembersForUpcomingSession(int sessionId, CancellationToken ct = default);
        Task<Booking?> GetBookingByMemberAndSessionAsync(int memberId, int sessionId, CancellationToken ct = default);
        Task<Member?> GetMemberWithMemberShip(int memberId, CancellationToken ct = default);
        Task<Booking?> GetBookingWithSession(int sessionId, CancellationToken ct = default);
    }
}
