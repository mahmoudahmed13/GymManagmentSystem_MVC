using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.BLL.ViewModels.MemberShipViewModels;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result> CreateBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(model.SessionId, ct);
            if (session == null) return Result.NotFound($"Session not found.");

            if(session.EndDate <= DateTime.UtcNow) return Result.Fail($"Session has already started or finished cannot be booked.", ResultKind.Conflict);

            if(session.Capacity <= await _unitOfWork.BookingRepository.GetCountOfBookedSlotsAsync(session.Id, ct))
                return Result.Fail($"Session is fully booked.", ResultKind.Conflict);

            var member = await _unitOfWork.BookingRepository.GetMemberWithMemberShip(model.MemberId, ct);
            if (member == null)return Result.NotFound($"Member not found.");

            if (!member.MemberShips.Any(ms => ms.IsActive))
                return Result.Fail($"Member does not have any active membership.", ResultKind.Conflict);

            if(member.Bookings.Any(b => b.SessionId == model.SessionId))
                return Result.Fail($"Member is already booked for this session.", ResultKind.Conflict);

            _unitOfWork.GetRepository<Booking>().Add(_mapper.Map<Booking>(model));
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail($"Failed to create booking.", ResultKind.Conflict);
        }
        public async Task<IEnumerable<MemberViewModelForBooking>> GetMembersForDropDownAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<MemberViewModelForBooking>>(members);
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync(ct:ct);
            if(sessions == null || !sessions.Any()) return null;

            var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.BookingRepository.GetCountOfBookedSlotsAsync(session.Id, ct);
            }
            return mappedSessions;
        }

        
        public async Task<Result<IEnumerable<SessionMembersViewModel>>> GetMembersForUpcomingSession(int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetMembersForUpcomingSession(sessionId, ct);
            if (bookings == null || !bookings.Any())
                return Result<IEnumerable<SessionMembersViewModel>>.NotFound("No bookings found.");
            var mapBooing = _mapper.Map<IEnumerable<SessionMembersViewModel>>(bookings);
            return Result<IEnumerable<SessionMembersViewModel>>.Ok(mapBooing);
        }

        public async Task<Result<IEnumerable<SessionMembersViewModel>>> GetMembersForOngoingSession(int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetMembersForOngoingSession(sessionId, ct);
            if (bookings == null || !bookings.Any())
                return Result<IEnumerable<SessionMembersViewModel>>.NotFound("No bookings found.");
            var mapBooing = _mapper.Map<IEnumerable<SessionMembersViewModel>>(bookings);
            return Result<IEnumerable<SessionMembersViewModel>>.Ok(mapBooing);
        }

        

        public async Task<IEnumerable<MemberViewModelForBooking>> GetMembersForDropDownAsync(int sessionId, CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<MemberViewModelForBooking>>(members);
        }

        public async Task<Result> MembersIsAttended(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking =await _unitOfWork.BookingRepository.GetBookingByMemberAndSessionAsync(memberId, sessionId, ct);
            if (booking == null)
                return Result.NotFound("Booking not found.");
            booking.IsAttended = true;
            _unitOfWork.BookingRepository.Update(booking);
            return await _unitOfWork.SaveChangesAsync(ct) > 0 ? Result.Ok() : Result.Fail("Failed to update booking.", ResultKind.Conflict);
        }

        public async Task<Result> RemoveMemberBooked(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.GetBookingByMemberAndSessionAsync(memberId, sessionId, ct);
            if (booking == null)
                return Result.NotFound("Booking not found.");
            if (booking.IsAttended)
                return Result.Fail("Cannot remove booking for a member who has already attended the session.", ResultKind.Conflict);
            var bookingSsssion = await _unitOfWork.BookingRepository.GetBookingWithSession(sessionId, ct);
            if (bookingSsssion.Session.StartDate <= DateTime.UtcNow)
                return Result.Fail("Cannot remove booking for a session that has already started.", ResultKind.Conflict);
            booking.IsAttended = false;
            _unitOfWork.BookingRepository.Delete(booking);
            return await _unitOfWork.SaveChangesAsync(ct) > 0 ? Result.Ok() : Result.Fail("Failed to remove booking.", ResultKind.Conflict);
        }
    }
}
