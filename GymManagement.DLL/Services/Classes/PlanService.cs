using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);

            var mappedPlan = _mapper.Map<IEnumerable<PlanViewModel>>(plans);
            return mappedPlan;
        }

        public async Task<Result<PlanViewModel>> GetPlanByIdAsync(int id, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return Result<PlanViewModel>.NotFound("Plan Not Found");
            
            var mappedPlan = _mapper.Map<Plan, PlanViewModel>(plan);
            return Result<PlanViewModel>.Ok(mappedPlan);
        }

        public async Task<Result<UpdatePlanViewModel>> GetPlanToUpdateAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan is null || !plan.IsActive) return Result<UpdatePlanViewModel>.NotFound("Plan Not Found");

            if (await HasActiveMembershipsAsync(planId, ct)) return Result<UpdatePlanViewModel>.Validation("Can Not Edit Plan Has Active Membership");
            else
            {
                var mpperPlan = _mapper.Map<UpdatePlanViewModel>(plan);
                return Result<UpdatePlanViewModel>.Ok(mpperPlan);
            }
        }

        public async Task<Result> ToggleActivationAsync(int planId, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId, ct);
            if (plan == null) return Result.NotFound("Plan Not Found");

            if (await HasActiveMembershipsAsync(planId, ct) && plan.IsActive) return Result.Fail("Can Not Deactivate Plan Has Active Membership");

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Remove");
        }

        public async Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan == null) return Result.NotFound("Plan Not Found");

            if (await HasActiveMembershipsAsync(id, ct)) return Result.Fail("Can Not Edit Plan Has Active Membership");

            _mapper.Map(model, plan);
            _unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Edit");
        }
        private async Task<bool> HasActiveMembershipsAsync(int planId, CancellationToken ct = default)
        {
            return await _unitOfWork.GetRepository<MemberShip>().AnyAsync(m => m.PlanId == planId && m.EndData > DateTime.Now, ct);
        }
    }
}
