using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Classes;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AnalyticsViewModel>> GetAnalyticsViewModelAsync(CancellationToken ct = default)
        {
            var now = DateTime.Now;
            var upcomingSessions = await _unitOfWork.SessionRepository.CountAsync(s => s.StartDate > now);
            var ongoingSessions = await _unitOfWork.SessionRepository.CountAsync(s => s.StartDate <= now && s.EndDate >= now );
            var completedSessions = await _unitOfWork.SessionRepository.CountAsync(s => s.EndDate <= now);

            var totalMembers = await _unitOfWork.GetRepository<Member>().CountAsync(ct:ct);
            var totalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(ct:ct);

            var activeMember = await _unitOfWork.GetRepository<MemberShip>().CountAsync(x => x.EndData > now, ct);

            var analtrics = new AnalyticsViewModel()
            {
                TotalMembers = totalMembers,
                TotalTrainers = totalTrainers,
                UpcomingSessions = upcomingSessions,
                CompletedSessions = completedSessions,
                OngoingSessions = ongoingSessions,
                ActiveMembers = activeMember,
            };

            return Result<AnalyticsViewModel>.Ok(analtrics);
        }

        //public async Task<Result<AnalyticsViewModel>> GetAnalyticsViewModelAsync(CancellationToken ct = default)
        //{
        //    var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();
        //    var activeMembers = await _unitOfWork.GetRepository<MemberShip>().GetAllAsync();
        //    var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();

        //    var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync();

        //    var upcomingSessions = 0;
        //    var ongoingSessions = 0;
        //    var completedSessions = 0;
        //    foreach (var session in sessions)
        //    {

        //        if (session.StartDate > DateTime.Now)
        //            ++upcomingSessions;
        //        else if (session.StartDate <= DateTime.Now && session.EndDate >= DateTime.Now)
        //            ++ongoingSessions;
        //        else
        //            ++completedSessions;
        //    }

        //    var analtrics = new AnalyticsViewModel()
        //    {
        //        TotalMembers = members.Count(),
        //        TotalTrainers = trainers.Count(),
        //        ActiveMembers = members.Count(),
        //        UpcomingSessions = upcomingSessions,
        //        OngoingSessions = ongoingSessions,
        //        CompletedSessions = completedSessions
        //    };
        //    return Result<AnalyticsViewModel>.Ok(analtrics);
        //}
    }
}
