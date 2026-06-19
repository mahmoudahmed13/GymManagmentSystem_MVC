using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.PlanViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagment.Controllers
{
    public class PlansController : Controller
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }
        //Index() - Plans listing page
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planService.GetAllPlansAsync(ct);
            return View(plans);
        }
        //Details(int id) - Plan details page
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planService.GetPlanByIdAsync(id, ct);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan not fount.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        //● Edit(int id) - Plan edit form
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
        {
            var plan = await _planService.GetPlanToUpdateAsync(id, ct);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan not fount.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);

        }

        //● Edit(int id) - Update handler
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
            if(!ModelState.IsValid) return View(model);

            var result = await _planService.UpdatePlanAsync(id, model, ct);
            if (result)
                TempData["SuccessMessage"] = "Plan updated successfully.";
            else
                TempData["ErrorMessage"] = "Plan Failed to update.";

            return RedirectToAction(nameof(Index));
        }

        //● Activate(int id) - Toggle plan active status
        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken ct = default)
        {
            var plan = await _planService.ToggleActivationAsync(id, ct);
            if (plan)
                TempData["SuccessMessage"] = "Plan updated successfully.";
            else
                TempData["ErrorMessage"] = "Plan Failed to update.";

            return RedirectToAction(nameof(Index));

        }

    }
}
