using GymManagement.BLL.ViewModels.MemberViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberServices
    {
        Task<IEnumerable<MemberViewModel>> GetAllMemberAsync(CancellationToken ct = default);

        Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default);
    }
}
