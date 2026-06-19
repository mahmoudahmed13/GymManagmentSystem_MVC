using GymManagement.BLL.ViewModels.MemberViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberServices
    {
        Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default);

        Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);

        Task<MemberViewModel?> GetMemberDetailsByIdAsync(int memberId, CancellationToken ct = default);

        Task<HealthRecordViewModel?> GetMemberHealthRecoedAsync(int memberId, CancellationToken ct = default);

        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default);

        Task<bool> UpdateMemberDetailsAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default);
        Task<bool> RemoveMemberAsync(int memberId , CancellationToken ct = default);
    }
}
