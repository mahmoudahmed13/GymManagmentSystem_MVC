using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class TrainersController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainersController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        public async Task<IActionResult> Index(CancellationToken ct = default)
            => View(await _trainerService.GetAllTrainersAsync(ct));
        [HttpGet]
        public IActionResult Create(CancellationToken ct = default)
            => View();
        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);

            var trainer = await _trainerService.CreateTrainerAsync(model, ct);
            if (trainer.success)
                TempData["SuccessMessage"] = "Trainer Created Successfully";
            else
                TempData["ErrorMessage"] = trainer.error;

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerService.GetTrainerDetailsByIdAsync(id, ct);
            if (trainer.success)
                return View(trainer.Value);

            else
            {
                TempData["ErrorMessage"] = trainer.error;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerService.GetTrainerToUpdateAsyn(id, ct);
            if (trainer.success)
                return View(trainer.Value);
            else
            {
                TempData["ErrorMessage"] = trainer.error;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);

            var trainer = await _trainerService.UpdateTrainerDetailsAsync(id, model, ct);
            if (trainer.success)
                TempData["SuccessMessage"] = "Trainer Edited successfully";
            else
                TempData["ErrorMessage"] = trainer.error;

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerService.GetTrainerToUpdateAsyn(id , ct);
            if (trainer.success)
                return View(trainer.Value);

            else
            {
                TempData["ErrorMessage"] = trainer.error;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int id, CancellationToken ct = default)
        {
            var trainer = await _trainerService.RemoveTrainerAsync(id, ct);
            if (trainer.success)
                TempData["SuccessMessage"] = "Trainer Deleted successfully";
            else
                TempData["ErrorMessage"] = trainer.error;

            return RedirectToAction(nameof(Index));
        } 

    }
}
