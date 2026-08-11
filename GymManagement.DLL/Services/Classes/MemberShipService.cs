using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberShipViewModels;
using GymManagement.DAL.Data.Models;
using GymManagement.DAL.Repositorities.Interfaces;

namespace GymManagement.BLL.Services.Classes
{
    public class MemberShipService : IMemberShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberShipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result> CancelMemberShipAsync(int memberShipId, CancellationToken ct = default)
        {
            var memberShips = await _unitOfWork.GetRepository<MemberShip>().GetByIdAsync(memberShipId, ct);
            if (memberShips == null) return Result.NotFound("Membership not found");

            if (memberShips.IsActive == false) return Result.Fail("Membership is already canceled", ResultKind.Conflict);
            _unitOfWork.GetRepository<MemberShip>().Delete(memberShips);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to cancel membership", ResultKind.Conflict);
        }

        public async Task<Result> CreateMemberShipAsync(CreateMemberShipViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(model.MemberId);
            if (member == null) return Result.NotFound("Member not found");

            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId);
            if (plan == null) return Result.NotFound("Plan not found");


            var memberShip = _mapper.Map<MemberShip>(model);

            memberShip.CreatedAt = DateTime.UtcNow;
            memberShip.EndData = DateTime.UtcNow.AddDays(plan.DurationDays);

            _unitOfWork.GetRepository<MemberShip>().Add(memberShip);    
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to create membership", ResultKind.Conflict);
        }

        public async Task<IEnumerable<MemberShipViewModel>?> GetAllMemberShipAsync(CancellationToken ct = default)
        {
            var memberShips = await _unitOfWork.MemberShipRepository.GetAllMemberShipWithMemberAndPlan(ct);
            return _mapper.Map<IEnumerable<MemberShipViewModel>>(memberShips);

        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(members);
        }

        public async Task<IEnumerable<PlanSelectViewModel>> GetPlansForDropDownAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<PlanSelectViewModel>>(plans);
        }
    }
}
