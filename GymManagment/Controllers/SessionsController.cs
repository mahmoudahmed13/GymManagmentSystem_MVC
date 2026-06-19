using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    public class SessionsController : Controller
    {
       private readonly ISessionService _sessionService;

        public SessionsController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }
        public async Task<ActionResult> Index(CancellationToken ct = default)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(sessions);
        }
    }
}
