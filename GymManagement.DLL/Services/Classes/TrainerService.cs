using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            
            return _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
        }

        public async Task<Result<TrainerViewModel>> GetTrainerDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null) return Result<TrainerViewModel>.NotFound("Tainer Not Found");
            else
            {
                var mapperTrianer = _mapper.Map<TrainerViewModel>(trainer);
                return Result<TrainerViewModel>.Ok(mapperTrianer);
            }
        }
        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var emailExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == model.Email, ct);
            //checkPhone            
            var phoneExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == model.Phone, ct);

            //Email or Phone Exist Return False
            if (emailExist || phoneExist) return Result.Fail("Email or Phone Already Exist"); ;

            var trainer = _mapper.Map<Trainer>(model);
            _unitOfWork.GetRepository<Trainer>().Add(trainer);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Create");
        }

        public async Task<Result<TrainerToUpdateViewModel>> GetTrainerToUpdateAsyn(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return Result<TrainerToUpdateViewModel>.NotFound("Tainer Not Found");
            else
            {
                var mappedTrainer = _mapper.Map<Trainer, TrainerToUpdateViewModel>(trainer);
                return Result<TrainerToUpdateViewModel>.Ok(mappedTrainer);
            }
        }
        public async Task<Result> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return Result.NotFound("Tainer Not Found");
            if (await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id != id, ct)) return Result.Validation("Email Must Be Unique");
            if(await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != id, ct)) return Result.Validation("Phone Must Be Unique");

            trainer.UpdatedAt = DateTime.Now;
            _mapper.Map(model, trainer);
            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Edit");
        }

        public async Task<Result> RemoveTrainerAsync(int id, CancellationToken ct = default)
        {
            
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return Result.NotFound("Tainer Not Found");

            var hasFutureSessions = await _unitOfWork.GetRepository<Session>().AnyAsync(s => s.TrainerId == id && s.StartDate > DateTime.Now, ct);
            if(hasFutureSessions) return Result.Validation("Can Not Delete Trainer Has Fustur Sessions");

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Delete");

        }

    }
}
