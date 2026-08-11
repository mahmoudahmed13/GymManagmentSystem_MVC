using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.BLL.ViewModels.SessionViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default);
        //Task<Result<IEnumerable<SessionMembersViewModel>>> GetMembersForBookedSessionAsync(int id, CancellationToken ct = default);
        Task<Result<IEnumerable<SessionMembersViewModel>>> GetMembersForUpcomingSession(int sessionId, CancellationToken ct = default);
        Task<Result<IEnumerable<SessionMembersViewModel>>> GetMembersForOngoingSession(int sessionId, CancellationToken ct = default);
        Task<Result> MembersIsAttended(int memberId, int sessionId, CancellationToken ct = default);
        Task<Result> CreateBookingAsync(CreateBookingViewModel model, CancellationToken ct = default);
        Task<IEnumerable<MemberViewModelForBooking>> GetMembersForDropDownAsync(int sessionId, CancellationToken ct = default);
        Task<Result> RemoveMemberBooked(int memberId, int sessionId, CancellationToken ct = default);
    }
}
