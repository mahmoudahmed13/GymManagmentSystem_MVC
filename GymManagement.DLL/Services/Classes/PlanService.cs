using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);

            //manual maping plans => PlanViewModel
            return plans.Select(planViewModel => new PlanViewModel()
            {
                Id = planViewModel.Id,
                Name = planViewModel.Name,
                Description = planViewModel.Description,
                DurationDays = planViewModel.DurationDays,
                IsActive = planViewModel.IsActive,
                Price = planViewModel.Price,
            });
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return null;

            else
                return new PlanViewModel()
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    DurationDays = plan.DurationDays,
                    IsActive = plan.IsActive,
                    Price = plan.Price,
                };
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan is null || !plan.IsActive) return null;

            if (await HasActiveMembershipsAsync(planId, ct)) return null;
            else
                return new UpdatePlanViewModel()
                {
                    PlanName = plan.Name,
                    Price = plan.Price,
                    Description = plan.Description,
                    DurationDays = plan.DurationDays,
                };
        }

        public async Task<bool> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan == null) return false;

            if (await HasActiveMembershipsAsync(planId, ct) && plan.IsActive) return false;

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }

        public async Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return false;

            if (await HasActiveMembershipsAsync(id, ct)) return false;

            plan.DurationDays = model.DurationDays;
            plan.Description = model.Description;
            plan.Price = model.Price;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }
        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<MemberShip>().AnyAsync(m => m.PlanId == planId && m.EndData > DateTime.Now, ct);
        }
    }
}
