using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagement.PL.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IBookingService _bookingService;
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var bookings = await _bookingService.GetAllSessionsAsync(ct);
            return View(bookings);
        }
        [HttpGet]
        public async Task<IActionResult> Create(int id, CancellationToken ct)
        {
            await PopulateDropDownsAsync(id, ct);
            var model = new CreateBookingViewModel
            {
                SessionId = id
            };

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) 
            {
                await PopulateDropDownsAsync(model.SessionId,ct);
                return View(model);
            }
            var result = await _bookingService.CreateBookingAsync(model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Booking created successfully.";
                return RedirectToAction(nameof(GetMembersForUpcomingSession), new { sessionId = model.SessionId });
            }

            TempData["ErrorMessage"] = result.error;
            await PopulateDropDownsAsync(model.SessionId, ct);
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetMembersForOngoingSession(int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.GetMembersForOngoingSession(sessionId, ct);
            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetMembersForUpcomingSession(int id, CancellationToken ct)
        {
            var result = await _bookingService.GetMembersForUpcomingSession(id, ct);

            return View(result.Value);
        }
        public async Task<IActionResult> Attended(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.MembersIsAttended(memberId, sessionId, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Attendance marked successfully." : result.error;
            return RedirectToAction(nameof(GetMembersForOngoingSession), new { sessionId });
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.RemoveMemberBooked(memberId, sessionId, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] = result.success ? "Booking removed successfully." : result.error;
            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { sessionId });
        }
        private async Task PopulateDropDownsAsync(int sessionId, CancellationToken ct)
        {
            var members = await _bookingService.GetMembersForDropDownAsync(sessionId, ct);
            ViewBag.Members = new SelectList(members, "Id", "Name");
        }
    }
}
