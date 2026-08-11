using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    [Authorize]
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

        #region Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDownListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownListAsync();
                return View(model);
            }

            var result = await _sessionService.CreateSessionAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Created";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.error;
            await PopulateDropDownListAsync();
            return View(model);

        }
        private async Task PopulateDropDownListAsync()
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetCategoriesForDropDownAsync(), "Id", "CategoryName");
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var result = await _sessionService.GetSessionByIdAsync(id, ct);
            if (result.success)
                return View(result.Value);
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }

        }

        #region Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
        {
            var result = await _sessionService.GetSessionToUpdateAsync(id, ct);
            if (result.success)
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(result.Value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) 
            {
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(model);
            }

            var result = await _sessionService.UpdateSessionAsync(id, model , ct);

            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Updated";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                ViewBag.Trainers = new SelectList(await _sessionService.GetTrainersForDropDownAsync(), "Id", "Name");
                return View(model);
            }

        }
        #endregion

        #region Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var result = await _sessionService.GetSessionByIdAsync(id);
            if(result.success)
                return View(result.Value);

            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct = default)
        {
            var result = await _sessionService.RemoveSessionAsync(id, ct);

            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "SessionDeleted" : result.error;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
