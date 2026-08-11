using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberShipViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    [Authorize]
    public class MembershipsController : Controller
    {
        private readonly IMemberShipService _memberShipService;

        public MembershipsController(IMemberShipService memberShipService)
        {
            _memberShipService = memberShipService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var memberships = await _memberShipService.GetAllMemberShipAsync(ct);
            return View(memberships);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateDropDownListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberShipViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropDownListAsync();
                return View(model);
            }
            var result = await _memberShipService.CreateMemberShipAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Membership Created";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            await PopulateDropDownListAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int Id, CancellationToken ct)
        {
            var result = await _memberShipService.CancelMemberShipAsync(Id, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Membership Canceled" : result.error;
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropDownListAsync()
        {
            ViewBag.Members = new SelectList(await _memberShipService.GetMembersForDropDownAsync(), "Id", "Name");
            ViewBag.Plans = new SelectList(await _memberShipService.GetPlansForDropDownAsync(), "Id", "Name");
        }
    }
}
