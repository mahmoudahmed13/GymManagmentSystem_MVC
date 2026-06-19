using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagement.PL.Controllers
{
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
            if (trainer)
                TempData["SuccessMessage"] = "Trainer Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Create Trainer";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerService.GetTrainerDetailsByIdAsync(id, ct);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Failed To Create Trainer";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerService.GetTrainerToUpdateAsyn(id, ct);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);

            var trainer = await _trainerService.UpdateTrainerDetailsAsync(id, model, ct);
            if (trainer)
                TempData["SuccessMessage"] = "Trainer Edited successfully";
            else
                TempData["ErrorMessage"] = "Failed To Edit Trainer";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var trainer = await _trainerService.GetTrainerToUpdateAsyn(id , ct);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Failed To Create Trainer";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int id, CancellationToken ct = default)
        {
            var trainer = await _trainerService.RemoveTrainerAsync(id, ct);
            if (trainer)
                TempData["SuccessMessage"] = "Trainer Deleted successfully";
            else
                TempData["ErrorMessage"] = "Failed To Delete Trainer";

            return RedirectToAction(nameof(Index));
        } 

    }
}
