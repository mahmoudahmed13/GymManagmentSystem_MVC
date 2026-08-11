using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MemberShipViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberShipService
    {
        Task<IEnumerable<MemberShipViewModel>?> GetAllMemberShipAsync(CancellationToken ct = default);
        Task<Result> CreateMemberShipAsync(CreateMemberShipViewModel model, CancellationToken ct = default);
        Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownAsync(CancellationToken ct = default);
        Task<IEnumerable<PlanSelectViewModel>> GetPlansForDropDownAsync(CancellationToken ct = default);
        Task<Result> CancelMemberShipAsync(int memberShipId, CancellationToken ct = default);
    }
}
