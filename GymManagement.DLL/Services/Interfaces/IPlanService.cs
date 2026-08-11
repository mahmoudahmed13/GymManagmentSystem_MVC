using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.PlanViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct =  default);
        Task<Result<PlanViewModel>> GetPlanByIdAsync(int id, CancellationToken ct = default);
        Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int planId, CancellationToken ct = default);
        Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);
        Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default);
    }
}
