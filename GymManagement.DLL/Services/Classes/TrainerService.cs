using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return trainers.Select(t => new TrainerViewModel()
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Phone = t.Phone,
                specialization = t.Specialty.ToString(),
            });
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer == null) return null;
            else
                return new TrainerViewModel()
                {
                    Id = trainer.Id,
                    Name = trainer.Name,
                    Email = trainer.Email,
                    Phone = trainer.Phone,
                    DateOfBirth = trainer.DateOfBirth.ToString(),
                    Address = $"{trainer.Address.BuildingNumber} - {trainer.Address.Street} - {trainer.Address.City}"
                };
        }
        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var emailExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Email == model.Email, ct);
            //checkPhone            
            var phoneExist = await _unitOfWork.GetRepository<Trainer>().AnyAsync(x => x.Phone == model.Phone, ct);

            //Email or Phone Exist Return False
            if (emailExist || phoneExist) return false;

            var trainer = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Specialty = model.specialization,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City
                },
            };

            _unitOfWork.GetRepository<Trainer>().Add(trainer);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsyn(int id, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return null;
            else
                return new TrainerToUpdateViewModel()
                {
                    Name = trainer.Name,
                    Email = trainer.Email,
                    Phone = trainer.Phone,
                    BuildingNumber = trainer.Address.BuildingNumber,
                    Street =  trainer.Address.Street,
                    City = trainer.Address.City,
                    Specialty = trainer.Specialty,
                };

        }
        public async Task<bool> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return false;
            if(await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email && t.Id != id, ct)) return false;
            if(await _unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone && t.Id != id, ct)) return false;

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;
            trainer.Specialty = model.Specialty;
            trainer.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<bool> RemoveTrainerAsync(int id, CancellationToken ct = default)
        {
            
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return false;

            var hasFutureSessions = await _unitOfWork.GetRepository<Session>().AnyAsync(s => s.TrainerId == id && s.StartDate > DateTime.Now, ct);
            if(hasFutureSessions) return false;

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;

        }

    }
}
