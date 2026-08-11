using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Data.Models.Enums;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("EndDate Must Be After StartDate");
            if (model.StartDate <= DateTime.Now) return Result.Validation("StartDate Must Be In The Future");
            if (model.Capacity < 1 || model.Capacity > 25) return Result.Validation("Capacity Must Be Between 1 and 25");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer is null) return Result.NotFound("Trainer Not Fount");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (category is null) return Result.NotFound("Category Not Fount");

            var isValid = Enum.TryParse<Specialty>(category.CategoryName, true, out var CategorySpecialty);
            if (!isValid || trainer.Specialty != CategorySpecialty) return Result.Validation("Can Not Create This Session To Trainer");

            var session = _mapper.Map<CreateSessionViewModel, Session>(model);

            _unitOfWork.GetRepository<Session>().Add(session);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create Session");
        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var sessionRepo = _unitOfWork.SessionRepository;
            var sessions = await sessionRepo.GetAllSessionWithTrainerAndCategory(ct);

            if (sessions == null || !sessions.Any()) return null;

            var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
            foreach (var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - await sessionRepo.GetCountOfBookedSlotsAsync(session.Id, ct);
            }
            return mappedSessions;
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);
        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetSessionWithTrainerAndCategory(sessionId, ct);
            if (session is null)
                return Result<SessionViewModel>.NotFound("Session Not Fount");
            else
            {
                var mappedSession = _mapper.Map<Session, SessionViewModel>(session);
                mappedSession.AvailableSlots = mappedSession.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
                return Result<SessionViewModel>.Ok(mappedSession);
            }
        }
        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var result = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }


        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session == null) return Result<UpdateSessionViewModel>.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now)
                return Result<UpdateSessionViewModel>.Fail("Can Not Update Session that has already started");

            var bookingCOunt = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if (bookingCOunt > 0) 
                return Result<UpdateSessionViewModel>.Fail("Can Not Update Session that has already Bookings");
                
            var mappedSession = _mapper.Map<UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(mappedSession);

        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session == null)
                return Result.NotFound("Session Not Found");
            if (session.EndDate <= DateTime.Now)
            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Can Not Edit Session that has already started");

            if (session.EndDate <= session.StartDate)
                return Result.Fail("End Date must by after Start Date");

            var bookingCOunt = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(id);
            if (bookingCOunt > 0) 
                return Result.Fail("Can Not Edit Session that has already Bookings");

            if (model.StartDate <= DateTime.Now)
                return Result.Validation("Start Date Must Be In Future");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer == null)
                return Result.Fail("TRainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);

            var isValid = Enum.TryParse<Specialty>(category?.CategoryName, true, out var CategorySpecialty);
            if (!isValid || trainer.Specialty != CategorySpecialty) return Result.Validation("Can Not Create This Session To Trainer");

            //_mapper.Map<UpdateSessionViewModel, Session>(model);
            _mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;
            _unitOfWork.SessionRepository.Update(session);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Fail To Udate Sessions");
        }

        public async Task<Result> RemoveSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session == null)
                return Result.NotFound("Session Is Not Found");

            if (session.EndDate >= DateTime.Now)
                return Result.Fail("Can Not Delete Session That Not Ended Yet");

            var bookingCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(sessionId, ct);
            if(bookingCount > 0)
                return Result.Fail("Can Not Delete Session That Has Bookings");

            _unitOfWork.SessionRepository.Delete(session);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete This Session");
        }
    }
}
