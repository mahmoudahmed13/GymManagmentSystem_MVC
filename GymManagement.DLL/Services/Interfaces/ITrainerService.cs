using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.TrainerViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default);

        Task<Result<TrainerViewModel>> GetTrainerDetailsByIdAsync(int id, CancellationToken ct = default); 

        Task<Result<TrainerToUpdateViewModel>> GetTrainerToUpdateAsyn(int id, CancellationToken ct = default);
        Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default);
        Task<Result> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default);
        Task<Result> RemoveTrainerAsync(int id, CancellationToken ct = default);
    }
}
