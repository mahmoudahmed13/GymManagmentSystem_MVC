using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.MemberViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberServices
    {
        Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default);

        Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);

        Task<Result<MemberViewModel>> GetMemberDetailsByIdAsync(int memberId, CancellationToken ct = default);

        Task<Result<HealthRecordViewModel>> GetMemberHealthRecoedAsync(int memberId, CancellationToken ct = default);

        Task<Result<MemberToUpdateViewModel>> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default);

        Task<Result> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default);
        Task<Result> RemoveMemberAsync(int memberId , CancellationToken ct = default);
    }
}
