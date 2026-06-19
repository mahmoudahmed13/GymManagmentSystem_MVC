using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.PL.Controllers
{

    public class MemberController : Controller
    {

        private readonly IMemberServices _memberService;
        public MemberController(IMemberServices memberService)
        {
            _memberService = memberService;
        }
        //Get BaseURL/Members/Index
        //Index() - Displays all member listing page
        public async Task<IActionResult> Index(CancellationToken ct = default)
        {
            var members = await _memberService.GetAllMemberAsync(ct);
            return View(members);
        }


        //Get BaseURL/Members/MemberDetails{id}
        //MemberDetails(int id) - Displays member profile page

        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct = default)
        {
            // Get Member By Id
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessager"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);

        }

        //Get BaseURL//Member/HealthRecordDetails/{id}
        //HealthRecordDetails(int id) - Shows health record page

        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct = default)
        {
            // Get HealthRecord By Id
            //Check Is HealthRecord Null => Retrun Index With Message
            //HealthRecord Is Not Null => Return View Data
            var result = await _memberService.GetMemberHealthRecoedAsync(id, ct);
            if (result is null)
            {
                TempData["ErrorMessager"] = "Health Record Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(result);
        }


        #region Create Member

        //Get BaseURL//Member/Create
        //Create() - Shows member registration form
        [HttpGet]
        public IActionResult Create() => View();

        //Post BaseURL//Member/CreateMember
        //CreateMember() - Save submitted form
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(nameof(Create), model);

            var result = await _memberService.CreateMemberAsync(model, ct);
            if (result)
                TempData["SuccessMessage"] = "Member Created Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Create Member";

            return RedirectToAction(nameof(Index));

        }
        #endregion

        #region Edit Member

        //Get BaseURL//Member/Edit/{id}
        //Edit(int id) - Displays edit form
        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberToUpdateAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessager"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(member);
        }

        //Post BaseURL//Member/Edit {Member}
        //Edit() - Save update
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _memberService.UpdateMemberDetailsAsync(id, model, ct);
            if (result)
                TempData["SuccessMessage"] = "Member Update Successfully";
            else
                TempData["ErrorMessager"] = "Failed To Update Member";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Delete Member
        //Get BaseUrl/Members/Delete/{id}
        //Delete(int id) - Shows deletion confirmation page

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var member = await _memberService.GetMemberDetailsByIdAsync(id, ct);
            if (member == null)
            {
                TempData["ErrorMessager"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        //Post BaseURL//Member/DeleteConfirm/{Id}
        //DeleteConfirm() - Processes deletion(submit From)

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct = default)
        {
            var result = await _memberService.RemoveMemberAsync(id, ct);
            if (result)
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed To Delete Member";

            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
