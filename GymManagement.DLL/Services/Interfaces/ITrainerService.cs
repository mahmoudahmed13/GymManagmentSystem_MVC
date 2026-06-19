using GymManagement.BLL.ViewModels.TrainerViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);

        Task<TrainerViewModel?> GetTrainerDetailsByIdAsync(int id, CancellationToken ct = default); 

        Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsyn(int id, CancellationToken ct = default);
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);
        Task<bool> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default);
        Task<bool> RemoveTrainerAsync(int id, CancellationToken ct = default);
    }
}
