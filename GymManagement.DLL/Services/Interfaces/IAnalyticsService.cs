using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<Result<AnalyticsViewModel>> GetAnalyticsViewModelAsync(CancellationToken ct = default);

    }
}
